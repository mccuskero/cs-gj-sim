using HEUSS.Core.Interfaces.Grains;
using HEUSS.Core.Models.Entities;
using Orleans;
using Orleans.Runtime;
using Orleans.Streams;

namespace HEUSS.Simulation.Grains.EntityGrains;

/// <summary>
/// Orleans grain representing a business/commercial building in the simulation.
/// Manages commercial energy consumption for HVAC, lighting, equipment, and operations.
/// </summary>
public class BusinessGrain : Grain, IBusinessGrain
{
    private Business? _state;
    private IAsyncStream<double>? _energyStream;
    private IPersistentState<Business>? _persistentState;
    private readonly Random _random = new();

    public BusinessGrain([PersistentState("business", "business-storage")] IPersistentState<Business> persistentState)
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
            _state = new Business
            {
                Id = this.GetPrimaryKey(),
                Name = $"Business-{this.GetPrimaryKey():N}",
                IndustryType = "Office",
                Workforce = 50,
                BuildingSizeSquareMeters = 500,
                DailyEnergyPerEmployee = 2_000_000 // ~0.5 kWh per employee per day
            };
        }

        // Set up energy stream to region
        var streamProvider = this.GetStreamProvider("EnergyStreamProvider");
        _energyStream = streamProvider.GetStream<double>(
            StreamId.Create("entity-energy", _state.RegionId));

        await base.OnActivateAsync(cancellationToken);
    }

    public Task<Business> GetStateAsync()
    {
        if (_state == null)
            throw new InvalidOperationException("Business grain state is not initialized");

        return Task.FromResult(_state);
    }

    public async Task SetStateAsync(Business business)
    {
        _state = business ?? throw new ArgumentNullException(nameof(business));

        if (_persistentState != null)
        {
            _persistentState.State = _state;
            await _persistentState.WriteStateAsync();
        }
    }

    public Task<double> GetEnergyUseAsync()
    {
        if (_state == null)
            throw new InvalidOperationException("Business grain state is not initialized");

        // Calculate base energy consumption
        var baseEnergy = _state.ComputeDailyEnergyUse();

        // Add stochastic variation (±10%) to simulate operational variability
        var variation = (_random.NextDouble() * 0.2) - 0.1; // -0.1 to +0.1
        var actualEnergy = baseEnergy * (1 + variation);

        return Task.FromResult(actualEnergy);
    }

    public async Task TickAsync()
    {
        if (_state == null || _energyStream == null)
            throw new InvalidOperationException("Business grain is not properly initialized");

        // Calculate daily energy consumption
        var energyConsumed = await GetEnergyUseAsync();

        // Publish to region energy stream
        await _energyStream.OnNextAsync(energyConsumed);
    }

    public Task<Guid> GetRegionIdAsync()
    {
        if (_state == null)
            throw new InvalidOperationException("Business grain state is not initialized");

        return Task.FromResult(_state.RegionId);
    }

    public async Task SetRegionIdAsync(Guid regionId)
    {
        if (_state == null)
            throw new InvalidOperationException("Business grain state is not initialized");

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
