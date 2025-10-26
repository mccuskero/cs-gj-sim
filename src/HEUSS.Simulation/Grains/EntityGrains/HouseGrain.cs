using HEUSS.Core.Interfaces.Grains;
using HEUSS.Core.Models.Entities;
using Orleans;
using Orleans.Runtime;
using Orleans.Streams;

namespace HEUSS.Simulation.Grains.EntityGrains;

/// <summary>
/// Orleans grain representing a house (residential building) in the simulation.
/// Manages technological energy consumption for HVAC, appliances, lighting, and water heating.
/// </summary>
public class HouseGrain : Grain, IHouseGrain
{
    private House? _state;
    private IAsyncStream<double>? _energyStream;
    private IPersistentState<House>? _persistentState;
    private readonly Random _random = new();

    public HouseGrain([PersistentState("house", "house-storage")] IPersistentState<House> persistentState)
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
            _state = new House
            {
                Id = this.GetPrimaryKey(),
                Name = $"House-{this.GetPrimaryKey():N}",
                SizeSquareMeters = 100,
                HouseholdCount = 2,
                HeatingType = Core.Enums.FuelType.NaturalGas,
                CoolingType = Core.Enums.FuelType.Electricity,
                InsulationFactor = 1.0
            };
            _state.InitializeTypicalEnergyConsumption(Core.Enums.ClimateType.Continental);
        }

        // Set up energy stream to region
        var streamProvider = this.GetStreamProvider("EnergyStreamProvider");
        _energyStream = streamProvider.GetStream<double>(
            StreamId.Create("entity-energy", _state.RegionId));

        await base.OnActivateAsync(cancellationToken);
    }

    public Task<House> GetStateAsync()
    {
        if (_state == null)
            throw new InvalidOperationException("House grain state is not initialized");

        return Task.FromResult(_state);
    }

    public async Task SetStateAsync(House house)
    {
        _state = house ?? throw new ArgumentNullException(nameof(house));

        if (_persistentState != null)
        {
            _persistentState.State = _state;
            await _persistentState.WriteStateAsync();
        }
    }

    public Task<double> GetEnergyUseAsync()
    {
        if (_state == null)
            throw new InvalidOperationException("House grain state is not initialized");

        // Calculate base energy consumption
        var baseEnergy = _state.ComputeDailyEnergyUse();

        // Add stochastic variation (±10%) to simulate weather, occupancy variations
        var variation = (_random.NextDouble() * 0.2) - 0.1; // -0.1 to +0.1
        var actualEnergy = baseEnergy * (1 + variation);

        return Task.FromResult(actualEnergy);
    }

    public async Task TickAsync()
    {
        if (_state == null || _energyStream == null)
            throw new InvalidOperationException("House grain is not properly initialized");

        // Calculate daily energy consumption
        var energyConsumed = await GetEnergyUseAsync();

        // Publish to region energy stream
        await _energyStream.OnNextAsync(energyConsumed);

        // State remains constant for now
        // Future: could adjust based on seasonal weather, occupancy changes, etc.
    }

    public Task<Guid> GetRegionIdAsync()
    {
        if (_state == null)
            throw new InvalidOperationException("House grain state is not initialized");

        return Task.FromResult(_state.RegionId);
    }

    public async Task SetRegionIdAsync(Guid regionId)
    {
        if (_state == null)
            throw new InvalidOperationException("House grain state is not initialized");

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
