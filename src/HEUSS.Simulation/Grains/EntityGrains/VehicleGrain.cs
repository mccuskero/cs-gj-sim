using HEUSS.Core.Interfaces.Grains;
using HEUSS.Core.Models.Entities;
using Orleans;
using Orleans.Runtime;
using Orleans.Streams;

namespace HEUSS.Simulation.Grains.EntityGrains;

/// <summary>
/// Orleans grain representing a vehicle in the simulation.
/// Manages transportation energy consumption for gasoline, electric, diesel, and hybrid vehicles.
/// </summary>
public class VehicleGrain : Grain, IVehicleGrain
{
    private Vehicle? _state;
    private IAsyncStream<double>? _energyStream;
    private IPersistentState<Vehicle>? _persistentState;
    private readonly Random _random = new();

    public VehicleGrain([PersistentState("vehicle", "vehicle-storage")] IPersistentState<Vehicle> persistentState)
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
            _state = new Vehicle
            {
                Id = this.GetPrimaryKey(),
                Name = $"Vehicle-{this.GetPrimaryKey():N}",
                FuelType = Core.Enums.FuelType.Gasoline,
                KilometersPerLiter = 12.0, // ~28 MPG
                DailyDistanceKm = 40.0 // Typical commute
            };
        }

        // Set up energy stream to region
        var streamProvider = this.GetStreamProvider("EnergyStreamProvider");
        _energyStream = streamProvider.GetStream<double>(
            StreamId.Create("entity-energy", _state.RegionId));

        await base.OnActivateAsync(cancellationToken);
    }

    public Task<Vehicle> GetStateAsync()
    {
        if (_state == null)
            throw new InvalidOperationException("Vehicle grain state is not initialized");

        return Task.FromResult(_state);
    }

    public async Task SetStateAsync(Vehicle vehicle)
    {
        _state = vehicle ?? throw new ArgumentNullException(nameof(vehicle));

        if (_persistentState != null)
        {
            _persistentState.State = _state;
            await _persistentState.WriteStateAsync();
        }
    }

    public Task<double> GetEnergyUseAsync()
    {
        if (_state == null)
            throw new InvalidOperationException("Vehicle grain state is not initialized");

        // Calculate base energy consumption
        var baseEnergy = _state.ComputeDailyEnergyUse();

        // Add stochastic variation (±15%) to simulate traffic, driving patterns
        var variation = (_random.NextDouble() * 0.3) - 0.15; // -0.15 to +0.15
        var actualEnergy = baseEnergy * (1 + variation);

        return Task.FromResult(actualEnergy);
    }

    public async Task TickAsync()
    {
        if (_state == null || _energyStream == null)
            throw new InvalidOperationException("Vehicle grain is not properly initialized");

        // Calculate daily energy consumption
        var energyConsumed = await GetEnergyUseAsync();

        // Publish to region energy stream
        await _energyStream.OnNextAsync(energyConsumed);

        // State remains constant for now
        // Future: could adjust daily distance based on day of week, seasonal patterns, etc.
    }

    public Task<Guid> GetRegionIdAsync()
    {
        if (_state == null)
            throw new InvalidOperationException("Vehicle grain state is not initialized");

        return Task.FromResult(_state.RegionId);
    }

    public async Task SetRegionIdAsync(Guid regionId)
    {
        if (_state == null)
            throw new InvalidOperationException("Vehicle grain state is not initialized");

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
