using Orleans;

namespace HEUSS.Core.Interfaces.Grains;

/// <summary>
/// Orleans grain interface for the temporal coordinator
/// Manages simulation time progression and tick broadcasting
/// </summary>
public interface ITemporalCoordinatorGrain : IGrainWithGuidKey
{
    /// <summary>
    /// Start the simulation with specified tick interval
    /// </summary>
    /// <param name="tickInterval">Time between simulation ticks</param>
    Task StartSimulationAsync(TimeSpan tickInterval);

    /// <summary>
    /// Stop the simulation
    /// </summary>
    Task StopSimulationAsync();

    /// <summary>
    /// Check if simulation is currently running
    /// </summary>
    Task<bool> IsRunningAsync();

    /// <summary>
    /// Get current simulation tick count
    /// </summary>
    Task<long> GetTickCountAsync();

    /// <summary>
    /// Get the tick interval
    /// </summary>
    Task<TimeSpan> GetTickIntervalAsync();

    /// <summary>
    /// Get simulation start time
    /// </summary>
    Task<DateTime> GetSimulationStartTimeAsync();

    /// <summary>
    /// Register a nation with the coordinator
    /// </summary>
    Task RegisterNationAsync(Guid nationId);

    /// <summary>
    /// Unregister a nation from the coordinator
    /// </summary>
    Task UnregisterNationAsync(Guid nationId);

    /// <summary>
    /// Get count of registered nations
    /// </summary>
    Task<int> GetNationCountAsync();

    /// <summary>
    /// Manually trigger a tick (for testing)
    /// </summary>
    Task ManualTickAsync();
}
