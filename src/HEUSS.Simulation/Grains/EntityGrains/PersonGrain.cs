using HEUSS.Core.Interfaces.Grains;
using HEUSS.Core.Models.Entities;
using Orleans;
using Orleans.Runtime;
using Orleans.Streams;

namespace HEUSS.Simulation.Grains.EntityGrains;

/// <summary>
/// Orleans grain representing an individual person in the simulation.
/// Manages biological energy consumption and publishes energy deltas to region stream.
/// </summary>
public class PersonGrain : Grain, IPersonGrain
{
    private Person? _state;
    private IAsyncStream<double>? _energyStream;
    private IPersistentState<Person>? _persistentState;
    private readonly Random _random = new();

    public PersonGrain([PersistentState("person", "person-storage")] IPersistentState<Person> persistentState)
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
            _state = new Person
            {
                Id = this.GetPrimaryKey(),
                Name = $"Person-{this.GetPrimaryKey():N}",
                Age = 30,
                Gender = "M",
                ActivityFactor = 1.5,
                SocioeconomicArchetype = Core.Enums.SocioeconomicArchetype.Middle,
                HasElectricityAccess = true,
                OwnsHouse = false,
                OwnsVehicle = false
            };
            _state.InitializeBasalMetabolicRate();
        }

        // Set up energy stream to region
        var streamProvider = this.GetStreamProvider("EnergyStreamProvider");
        _energyStream = streamProvider.GetStream<double>(
            StreamId.Create("entity-energy", _state.RegionId));

        await base.OnActivateAsync(cancellationToken);
    }

    public Task<Person> GetStateAsync()
    {
        if (_state == null)
            throw new InvalidOperationException("Person grain state is not initialized");

        return Task.FromResult(_state);
    }

    public async Task SetStateAsync(Person person)
    {
        _state = person ?? throw new ArgumentNullException(nameof(person));

        if (_persistentState != null)
        {
            _persistentState.State = _state;
            await _persistentState.WriteStateAsync();
        }
    }

    public Task<double> GetEnergyUseAsync()
    {
        if (_state == null)
            throw new InvalidOperationException("Person grain state is not initialized");

        // Calculate base biological energy
        var baseEnergy = _state.ComputeDailyEnergyUse();

        // Add stochastic variation (±5%) to simulate real-world variability
        var variation = (_random.NextDouble() * 0.1) - 0.05; // -0.05 to +0.05
        var actualEnergy = baseEnergy * (1 + variation);

        return Task.FromResult(actualEnergy);
    }

    public async Task TickAsync()
    {
        if (_state == null || _energyStream == null)
            throw new InvalidOperationException("Person grain is not properly initialized");

        // Calculate daily energy consumption
        var energyConsumed = await GetEnergyUseAsync();

        // Publish to region energy stream
        await _energyStream.OnNextAsync(energyConsumed);

        // Optionally update state based on simulation tick
        // (e.g., aging, activity changes, etc.)
        // For now, state remains constant between ticks
    }

    public Task<Guid> GetRegionIdAsync()
    {
        if (_state == null)
            throw new InvalidOperationException("Person grain state is not initialized");

        return Task.FromResult(_state.RegionId);
    }

    public async Task SetRegionIdAsync(Guid regionId)
    {
        if (_state == null)
            throw new InvalidOperationException("Person grain state is not initialized");

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
