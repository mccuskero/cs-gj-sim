using HEUSS.Core.Interfaces.Grains;
using HEUSS.Core.Models.Entities;
using Orleans;
using Orleans.Runtime;
using Orleans.Streams;

namespace HEUSS.Simulation.Grains.EntityGrains;

/// <summary>
/// Orleans grain representing a farm in the simulation.
/// Manages agricultural energy consumption AND food energy production.
/// Unique entity that both consumes and produces energy.
/// </summary>
public class FarmGrain : Grain, IFarmGrain
{
    private Farm? _state;
    private IAsyncStream<double>? _energyStream;
    private IPersistentState<Farm>? _persistentState;
    private readonly Random _random = new();

    public FarmGrain([PersistentState("farm", "farm-storage")] IPersistentState<Farm> persistentState)
    {
        _persistentState = persistentState;
    }

    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        // Load persisted state
        if (_persistentState?.State != null)
        {
            _state = _persistentState.State;
        }
        else
        {
            // Initialize default state if no persisted state exists
            _state = new Farm
            {
                Id = this.GetPrimaryKey(),
                Name = $"Farm-{this.GetPrimaryKey():N}",
                LandAreaHectares = 100,
                FarmType = "Mixed Crops",
                MachineryCount = 5,
                DailyFuelConsumptionLiters = 50, // ~50 liters diesel/day
                DailyElectricityKWh = 30, // ~30 kWh/day for irrigation, processing
                FoodOutputEnergyPerDay = 400_000_000, // 400 MJ/day food output
                WasteRate = 0.25 // 25% waste
            };
        }

        // Set up energy stream to region
        var streamProvider = this.GetStreamProvider("EnergyStreamProvider");
        _energyStream = streamProvider.GetStream<double>(
            StreamId.Create("entity-energy", _state.RegionId));

        await base.OnActivateAsync(cancellationToken);
    }

    public Task<Farm> GetStateAsync()
    {
        if (_state == null)
            throw new InvalidOperationException("Farm grain state is not initialized");

        return Task.FromResult(_state);
    }

    public async Task SetStateAsync(Farm farm)
    {
        _state = farm ?? throw new ArgumentNullException(nameof(farm));

        if (_persistentState != null)
        {
            _persistentState.State = _state;
            await _persistentState.WriteStateAsync();
        }
    }

    public Task<double> GetEnergyUseAsync()
    {
        if (_state == null)
            throw new InvalidOperationException("Farm grain state is not initialized");

        // Calculate base energy consumption (operational costs)
        var baseEnergy = _state.ComputeDailyEnergyUse();

        // Add stochastic variation (±15%) to simulate seasonal, weather variations
        var variation = (_random.NextDouble() * 0.3) - 0.15; // -0.15 to +0.15
        var actualEnergy = baseEnergy * (1 + variation);

        return Task.FromResult(actualEnergy);
    }

    public Task<double> GetFoodOutputEnergyAsync()
    {
        if (_state == null)
            throw new InvalidOperationException("Farm grain state is not initialized");

        // Net food energy after waste
        var netFoodEnergy = _state.ComputeNetFoodEnergyProduced();

        // Add stochastic variation (±20%) to simulate crop yield variations
        var variation = (_random.NextDouble() * 0.4) - 0.2; // -0.2 to +0.2
        var actualFoodEnergy = netFoodEnergy * (1 + variation);

        return Task.FromResult(actualFoodEnergy);
    }

    public Task<double> GetEnergyReturnOnInvestmentAsync()
    {
        if (_state == null)
            throw new InvalidOperationException("Farm grain state is not initialized");

        return Task.FromResult(_state.EnergyReturnOnInvestment);
    }

    public async Task TickAsync()
    {
        if (_state == null || _energyStream == null)
            throw new InvalidOperationException("Farm grain is not properly initialized");

        // Calculate daily energy consumption (operational costs)
        var energyConsumed = await GetEnergyUseAsync();

        // Publish operational energy consumption to region energy stream
        await _energyStream.OnNextAsync(energyConsumed);

        // Note: Food energy production is tracked separately and not included in regional energy consumption
        // Food energy feeds into the biological energy of Person entities
    }

    public Task<Guid> GetRegionIdAsync()
    {
        if (_state == null)
            throw new InvalidOperationException("Farm grain state is not initialized");

        return Task.FromResult(_state.RegionId);
    }

    public async Task SetRegionIdAsync(Guid regionId)
    {
        if (_state == null)
            throw new InvalidOperationException("Farm grain state is not initialized");

        _state.RegionId = regionId;

        // Update stream to new region
        var streamProvider = this.GetStreamProvider("EnergyStreamProvider");
        _energyStream = streamProvider.GetStream<double>(
            StreamId.Create("entity-energy", regionId));

        if (_persistentState != null)
        {
            _persistentState.State = _state;
            await _persistentState.WriteStateAsync();
        }
    }

    public override async Task OnDeactivateAsync(DeactivationReason reason, CancellationToken cancellationToken)
    {
        // Persist state before deactivation
        if (_persistentState != null && _state != null)
        {
            _persistentState.State = _state;
            await _persistentState.WriteStateAsync();
        }

        await base.OnDeactivateAsync(reason, cancellationToken);
    }
}
