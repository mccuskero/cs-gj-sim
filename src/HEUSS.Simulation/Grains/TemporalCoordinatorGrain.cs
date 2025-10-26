using HEUSS.Core.Interfaces.Grains;
using Orleans;
using Orleans.Runtime;
using Orleans.Timers;

namespace HEUSS.Simulation.Grains;

/// <summary>
/// Singleton Orleans grain that manages simulation time and tick coordination.
/// Uses Orleans Reminders to broadcast tick events to all active nations/regions.
/// </summary>
public class TemporalCoordinatorGrain : Grain, ITemporalCoordinatorGrain, IRemindable
{
    private const string TICK_REMINDER = "SimulationTick";

    private bool _isRunning;
    private long _tickCount;
    private TimeSpan _tickInterval;
    private DateTime _simulationStartTime;
    private readonly HashSet<Guid> _nationIds = new();

    public override Task OnActivateAsync(CancellationToken cancellationToken)
    {
        _isRunning = false;
        _tickCount = 0;
        _tickInterval = TimeSpan.FromHours(24); // Default: 1 day per tick
        _simulationStartTime = DateTime.UtcNow;

        return base.OnActivateAsync(cancellationToken);
    }

    public async Task StartSimulationAsync(TimeSpan tickInterval)
    {
        _isRunning = true;
        _tickInterval = tickInterval;
        _simulationStartTime = DateTime.UtcNow;
        _tickCount = 0;

        // Register Orleans Reminder to trigger ticks
        await this.RegisterOrUpdateReminder(
            TICK_REMINDER,
            dueTime: TimeSpan.Zero,      // Start immediately
            period: tickInterval);        // Repeat at interval
    }

    public async Task StopSimulationAsync()
    {
        _isRunning = false;

        // Unregister the reminder
        var reminder = await this.GetReminder(TICK_REMINDER);
        if (reminder != null)
        {
            await this.UnregisterReminder(reminder);
        }
    }

    public Task<bool> IsRunningAsync()
    {
        return Task.FromResult(_isRunning);
    }

    public Task<long> GetTickCountAsync()
    {
        return Task.FromResult(_tickCount);
    }

    public Task<TimeSpan> GetTickIntervalAsync()
    {
        return Task.FromResult(_tickInterval);
    }

    public Task<DateTime> GetSimulationStartTimeAsync()
    {
        return Task.FromResult(_simulationStartTime);
    }

    public Task RegisterNationAsync(Guid nationId)
    {
        _nationIds.Add(nationId);
        return Task.CompletedTask;
    }

    public Task UnregisterNationAsync(Guid nationId)
    {
        _nationIds.Remove(nationId);
        return Task.CompletedTask;
    }

    public Task<int> GetNationCountAsync()
    {
        return Task.FromResult(_nationIds.Count);
    }

    public async Task ReceiveReminder(string reminderName, TickStatus status)
    {
        if (reminderName != TICK_REMINDER || !_isRunning)
            return;

        try
        {
            // Increment tick count
            _tickCount++;

            // Broadcast tick to all registered nations
            var tasks = new List<Task>();

            foreach (var nationId in _nationIds)
            {
                var nationGrain = GrainFactory.GetGrain<INationGrain>(nationId);
                tasks.Add(nationGrain.TickAsync());
            }

            // Wait for all nations to complete their ticks
            await Task.WhenAll(tasks);
        }
        catch (Exception ex)
        {
            // Log error but don't stop simulation
            // TODO: Add proper logging
            Console.WriteLine($"Error during tick {_tickCount}: {ex.Message}");
        }
    }

    public async Task ManualTickAsync()
    {
        if (!_isRunning)
            throw new InvalidOperationException("Cannot manually tick when simulation is not running");

        // Manually trigger a tick (useful for testing)
        await ReceiveReminder(TICK_REMINDER, new Orleans.Runtime.TickStatus());
    }
}
