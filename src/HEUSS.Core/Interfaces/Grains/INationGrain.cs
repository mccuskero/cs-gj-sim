using HEUSS.Core.Models;

namespace HEUSS.Core.Interfaces.Grains;

/// <summary>
/// Orleans grain interface for Nation entity
/// Subscribes to region energy streams and aggregates national totals
/// </summary>
public interface INationGrain : Orleans.IGrainWithGuidKey
{
    /// <summary>
    /// Get the current state of the nation
    /// </summary>
    Task<Nation> GetStateAsync();

    /// <summary>
    /// Update nation state
    /// </summary>
    Task SetStateAsync(Nation nation);

    /// <summary>
    /// Register a region in this nation
    /// </summary>
    Task RegisterRegionAsync(Guid regionId);

    /// <summary>
    /// Get total energy consumption for this nation
    /// </summary>
    Task<double> GetTotalEnergyAsync();

    /// <summary>
    /// Get per capita energy consumption
    /// </summary>
    Task<double> GetPerCapitaEnergyAsync();

    /// <summary>
    /// Process a simulation tick
    /// </summary>
    Task TickAsync();
}
