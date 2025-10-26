using HEUSS.Core.Interfaces.Grains;
using HEUSS.Core.Models;
using Orleans;
using Orleans.Runtime;
using Orleans.Streams;

namespace HEUSS.Simulation.Grains;

/// <summary>
/// Orleans grain representing a geographic region (state, province, county).
/// Aggregates energy consumption from all entities within its boundaries.
/// Publishes regional totals to NationGrain via Orleans Streams.
/// </summary>
public class RegionGrain : Grain, IRegionGrain
{
    private Region? _state;
    private double _totalEnergyConsumed;
    private IAsyncStream<double>? _nationStream;
    private StreamSubscriptionHandle<double>? _entityStreamSubscription;
    private IPersistentState<Region>? _persistentState;

    // Track registered entities
    private readonly HashSet<Guid> _personIds = new();
    private readonly HashSet<Guid> _houseIds = new();
    private readonly HashSet<Guid> _vehicleIds = new();
    private readonly HashSet<Guid> _businessIds = new();
    private readonly HashSet<Guid> _dataCenterIds = new();
    private readonly HashSet<Guid> _farmIds = new();

    public RegionGrain([PersistentState("region", "region-storage")] IPersistentState<Region> persistentState)
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
            _state = new Region
            {
                Id = this.GetPrimaryKey(),
                Name = $"Region-{this.GetPrimaryKey():N}",
                Population = 0,
                DevelopmentLevel = Core.Enums.DevelopmentLevel.Developed,
                HasElectricity = true,
                ElectricityCoverage = 100.0,
                HasRunningWater = true,
                HasRoadNetwork = true,
                ClimateType = Core.Enums.ClimateType.Continental
            };
        }

        _totalEnergyConsumed = 0;

        // Set up stream to nation
        var streamProvider = this.GetStreamProvider("EnergyStreamProvider");
        _nationStream = streamProvider.GetStream<double>(
            StreamId.Create("region-energy", _state.NationId));

        // Subscribe to entity energy stream
        var entityStream = streamProvider.GetStream<double>(
            StreamId.Create("entity-energy", this.GetPrimaryKey()));

        _entityStreamSubscription = await entityStream.SubscribeAsync(
            OnEntityEnergyReceived);

        await base.OnActivateAsync(cancellationToken);
    }

    private async Task OnEntityEnergyReceived(double energyJoules, StreamSequenceToken token)
    {
        // Accumulate energy from entities
        _totalEnergyConsumed += energyJoules;
    }

    public Task<Region> GetStateAsync()
    {
        if (_state == null)
            throw new InvalidOperationException("Region grain state is not initialized");

        return Task.FromResult(_state);
    }

    public async Task SetStateAsync(Region region)
    {
        _state = region ?? throw new ArgumentNullException(nameof(region));

        if (_persistentState != null)
        {
            _persistentState.State = _state;
            await _persistentState.WriteStateAsync();
        }
    }

    public Task<double> GetTotalEnergyAsync()
    {
        return Task.FromResult(_totalEnergyConsumed);
    }

    public Task RegisterPersonAsync(Guid personId)
    {
        _personIds.Add(personId);
        return Task.CompletedTask;
    }

    public Task RegisterHouseAsync(Guid houseId)
    {
        _houseIds.Add(houseId);
        return Task.CompletedTask;
    }

    public Task RegisterVehicleAsync(Guid vehicleId)
    {
        _vehicleIds.Add(vehicleId);
        return Task.CompletedTask;
    }

    public Task RegisterBusinessAsync(Guid businessId)
    {
        _businessIds.Add(businessId);
        return Task.CompletedTask;
    }

    public Task RegisterDataCenterAsync(Guid dataCenterId)
    {
        _dataCenterIds.Add(dataCenterId);
        return Task.CompletedTask;
    }

    public Task RegisterFarmAsync(Guid farmId)
    {
        _farmIds.Add(farmId);
        return Task.CompletedTask;
    }

    public async Task TickAsync()
    {
        if (_state == null || _nationStream == null)
            throw new InvalidOperationException("Region grain is not properly initialized");

        // Tick all registered entities
        var tasks = new List<Task>();

        // Tick persons
        foreach (var personId in _personIds)
        {
            var personGrain = GrainFactory.GetGrain<IPersonGrain>(personId);
            tasks.Add(personGrain.TickAsync());
        }

        // Tick houses
        foreach (var houseId in _houseIds)
        {
            var houseGrain = GrainFactory.GetGrain<IHouseGrain>(houseId);
            tasks.Add(houseGrain.TickAsync());
        }

        // Tick vehicles
        foreach (var vehicleId in _vehicleIds)
        {
            var vehicleGrain = GrainFactory.GetGrain<IVehicleGrain>(vehicleId);
            tasks.Add(vehicleGrain.TickAsync());
        }

        // Tick businesses
        foreach (var businessId in _businessIds)
        {
            var businessGrain = GrainFactory.GetGrain<IBusinessGrain>(businessId);
            tasks.Add(businessGrain.TickAsync());
        }

        // Tick data centers
        foreach (var dataCenterId in _dataCenterIds)
        {
            var dataCenterGrain = GrainFactory.GetGrain<IDataCenterGrain>(dataCenterId);
            tasks.Add(dataCenterGrain.TickAsync());
        }

        // Tick farms
        foreach (var farmId in _farmIds)
        {
            var farmGrain = GrainFactory.GetGrain<IFarmGrain>(farmId);
            tasks.Add(farmGrain.TickAsync());
        }

        // Wait for all entities to tick
        await Task.WhenAll(tasks);

        // Give a small delay to allow stream messages to arrive
        await Task.Delay(100);

        // Publish regional total to nation stream
        await _nationStream.OnNextAsync(_totalEnergyConsumed);

        // Update region state
        if (_state != null)
        {
            _state.Population = _personIds.Count;
        }

        // Reset for next tick
        _totalEnergyConsumed = 0;
    }

    public Task<int> GetEntityCountAsync()
    {
        var total = _personIds.Count + _houseIds.Count + _vehicleIds.Count +
                   _businessIds.Count + _dataCenterIds.Count + _farmIds.Count;
        return Task.FromResult(total);
    }

    public Task<Guid> GetNationIdAsync()
    {
        if (_state == null)
            throw new InvalidOperationException("Region grain state is not initialized");

        return Task.FromResult(_state.NationId);
    }

    public async Task SetNationIdAsync(Guid nationId)
    {
        if (_state == null)
            throw new InvalidOperationException("Region grain state is not initialized");

        _state.NationId = nationId;

        // Update stream to new nation
        var streamProvider = this.GetStreamProvider("EnergyStreamProvider");
        _nationStream = streamProvider.GetStream<double>(
            StreamId.Create("region-energy", nationId));

        if (_persistentState != null)
        {
            _persistentState.State = _state;
            await _persistentState.WriteStateAsync();
        }
    }

    public override async Task OnDeactivateAsync(DeactivationReason reason, CancellationToken cancellationToken)
    {
        // Unsubscribe from entity stream
        if (_entityStreamSubscription != null)
        {
            await _entityStreamSubscription.UnsubscribeAsync();
        }

        // Persist state before deactivation
        if (_persistentState != null && _state != null)
        {
            _persistentState.State = _state;
            await _persistentState.WriteStateAsync();
        }

        await base.OnDeactivateAsync(reason, cancellationToken);
    }
}
