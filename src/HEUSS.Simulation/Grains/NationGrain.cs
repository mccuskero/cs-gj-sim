using HEUSS.Core.Interfaces.Grains;
using HEUSS.Core.Models;
using Orleans;
using Orleans.Runtime;
using Orleans.Streams;

namespace HEUSS.Simulation.Grains;

/// <summary>
/// Orleans grain representing a nation.
/// Aggregates energy consumption from all regions within the nation.
/// Publishes to analytics projections for dashboards.
/// </summary>
public class NationGrain : Grain, INationGrain
{
    private Nation? _state;
    private double _totalEnergyConsumed;
    private IAsyncStream<double>? _analyticsStream;
    private StreamSubscriptionHandle<double>? _regionStreamSubscription;
    private IPersistentState<Nation>? _persistentState;

    // Track registered regions
    private readonly HashSet<Guid> _regionIds = new();
    private readonly Dictionary<Guid, double> _regionEnergyMap = new();

    public NationGrain([PersistentState("nation", "nation-storage")] IPersistentState<Nation> persistentState)
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
            _state = new Nation
            {
                Id = this.GetPrimaryKey(),
                Name = $"Nation-{this.GetPrimaryKey():N}",
                CountryCode = "UNK",
                Population = 0,
                GdpPerCapita = 0
            };
        }

        _totalEnergyConsumed = 0;

        // Set up stream to analytics
        var streamProvider = this.GetStreamProvider("EnergyStreamProvider");
        _analyticsStream = streamProvider.GetStream<double>(
            StreamId.Create("nation-energy", this.GetPrimaryKey()));

        // Subscribe to region energy stream
        var regionStream = streamProvider.GetStream<double>(
            StreamId.Create("region-energy", this.GetPrimaryKey()));

        _regionStreamSubscription = await regionStream.SubscribeAsync(
            OnRegionEnergyReceived);

        await base.OnActivateAsync(cancellationToken);
    }

    private async Task OnRegionEnergyReceived(double energyJoules, StreamSequenceToken token)
    {
        // Accumulate energy from regions
        _totalEnergyConsumed += energyJoules;
    }

    public Task<Nation> GetStateAsync()
    {
        if (_state == null)
            throw new InvalidOperationException("Nation grain state is not initialized");

        return Task.FromResult(_state);
    }

    public async Task SetStateAsync(Nation nation)
    {
        _state = nation ?? throw new ArgumentNullException(nameof(nation));

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

    public Task<double> GetPerCapitaEnergyAsync()
    {
        if (_state == null || _state.Population == 0)
            return Task.FromResult(0.0);

        return Task.FromResult(_totalEnergyConsumed / _state.Population);
    }

    public Task RegisterRegionAsync(Guid regionId)
    {
        _regionIds.Add(regionId);
        return Task.CompletedTask;
    }

    public async Task TickAsync()
    {
        if (_state == null || _analyticsStream == null)
            throw new InvalidOperationException("Nation grain is not properly initialized");

        // Tick all registered regions
        var tasks = new List<Task>();

        foreach (var regionId in _regionIds)
        {
            var regionGrain = GrainFactory.GetGrain<IRegionGrain>(regionId);
            tasks.Add(regionGrain.TickAsync());
        }

        // Wait for all regions to tick
        await Task.WhenAll(tasks);

        // Give a small delay to allow stream messages to arrive
        await Task.Delay(200);

        // Publish national total to analytics stream
        await _analyticsStream.OnNextAsync(_totalEnergyConsumed);

        // Update nation population from regions
        if (_state != null)
        {
            long totalPopulation = 0;
            foreach (var regionId in _regionIds)
            {
                var regionGrain = GrainFactory.GetGrain<IRegionGrain>(regionId);
                var region = await regionGrain.GetStateAsync();
                totalPopulation += region.Population;
            }
            _state.Population = totalPopulation;
        }

        // Reset for next tick
        _totalEnergyConsumed = 0;
    }

    public Task<int> GetRegionCountAsync()
    {
        return Task.FromResult(_regionIds.Count);
    }

    public async Task<Dictionary<Guid, double>> GetRegionEnergyBreakdownAsync()
    {
        var breakdown = new Dictionary<Guid, double>();

        foreach (var regionId in _regionIds)
        {
            var regionGrain = GrainFactory.GetGrain<IRegionGrain>(regionId);
            var energy = await regionGrain.GetTotalEnergyAsync();
            breakdown[regionId] = energy;
        }

        return breakdown;
    }

    public override async Task OnDeactivateAsync(DeactivationReason reason, CancellationToken cancellationToken)
    {
        // Unsubscribe from region stream
        if (_regionStreamSubscription != null)
        {
            await _regionStreamSubscription.UnsubscribeAsync();
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
