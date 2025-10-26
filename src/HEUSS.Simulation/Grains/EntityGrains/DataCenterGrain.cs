using HEUSS.Core.Interfaces.Grains;
using HEUSS.Core.Models.Entities;
using Orleans;
using Orleans.Runtime;
using Orleans.Streams;

namespace HEUSS.Simulation.Grains.EntityGrains;

/// <summary>
/// Orleans grain representing a data center in the simulation.
/// Manages high-density energy consumption with PUE calculations.
/// </summary>
public class DataCenterGrain : Grain, IDataCenterGrain
{
    private DataCenter? _state;
    private IAsyncStream<double>? _energyStream;
    private IPersistentState<DataCenter>? _persistentState;
    private readonly Random _random = new();

    public DataCenterGrain([PersistentState("datacenter", "datacenter-storage")] IPersistentState<DataCenter> persistentState)
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
            _state = new DataCenter
            {
                Id = this.GetPrimaryKey(),
                Name = $"DataCenter-{this.GetPrimaryKey():N}",
                ServerCount = 1000,
                PowerUsageEffectiveness = 1.5, // Average PUE
                AveragePowerPerServer = 500 // watts
            };
        }

        // Set up energy stream to region
        var streamProvider = this.GetStreamProvider("EnergyStreamProvider");
        _energyStream = streamProvider.GetStream<double>(
            StreamId.Create("entity-energy", _state.RegionId));

        await base.OnActivateAsync(cancellationToken);
    }

    public Task<DataCenter> GetStateAsync()
    {
        if (_state == null)
            throw new InvalidOperationException("DataCenter grain state is not initialized");

        return Task.FromResult(_state);
    }

    public async Task SetStateAsync(DataCenter dataCenter)
    {
        _state = dataCenter ?? throw new ArgumentNullException(nameof(dataCenter));

        if (_persistentState != null)
        {
            _persistentState.State = _state;
            await _persistentState.WriteStateAsync();
        }
    }

    public Task<double> GetEnergyUseAsync()
    {
        if (_state == null)
            throw new InvalidOperationException("DataCenter grain state is not initialized");

        // Calculate base energy consumption
        var baseEnergy = _state.ComputeDailyEnergyUse();

        // Add stochastic variation (±5%) - data centers are more consistent
        var variation = (_random.NextDouble() * 0.1) - 0.05; // -0.05 to +0.05
        var actualEnergy = baseEnergy * (1 + variation);

        return Task.FromResult(actualEnergy);
    }

    public async Task TickAsync()
    {
        if (_state == null || _energyStream == null)
            throw new InvalidOperationException("DataCenter grain is not properly initialized");

        // Calculate daily energy consumption
        var energyConsumed = await GetEnergyUseAsync();

        // Publish to region energy stream
        await _energyStream.OnNextAsync(energyConsumed);
    }

    public Task<Guid> GetRegionIdAsync()
    {
        if (_state == null)
            throw new InvalidOperationException("DataCenter grain state is not initialized");

        return Task.FromResult(_state.RegionId);
    }

    public async Task SetRegionIdAsync(Guid regionId)
    {
        if (_state == null)
            throw new InvalidOperationException("DataCenter grain state is not initialized");

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
