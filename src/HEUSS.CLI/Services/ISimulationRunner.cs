using HEUSS.CLI.Models;

namespace HEUSS.CLI.Services;

/// <summary>
/// Interface for running energy simulations at different scopes
/// </summary>
public interface ISimulationRunner
{
    /// <summary>
    /// Run simulation globally for all nations
    /// </summary>
    Task<List<EnergyReport>> RunGlobalAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Run simulation for a specific nation
    /// </summary>
    Task<EnergyReport> RunNationAsync(string countryCode, CancellationToken cancellationToken = default);

    /// <summary>
    /// Run simulation for a specific region within a nation
    /// </summary>
    Task<EnergyReport> RunRegionAsync(string countryCode, string regionName, CancellationToken cancellationToken = default);
}
