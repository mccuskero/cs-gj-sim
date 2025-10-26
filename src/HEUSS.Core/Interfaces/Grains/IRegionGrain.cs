using HEUSS.Core.Models;

namespace HEUSS.Core.Interfaces.Grains;

/// <summary>
/// Orleans grain interface for Region entity
/// Manages entity grain activations and aggregates energy consumption
/// </summary>
public interface IRegionGrain : Orleans.IGrainWithGuidKey
{
    /// <summary>
    /// Get the current state of the region
    /// </summary>
    Task<Region> GetStateAsync();

    /// <summary>
    /// Update region state
    /// </summary>
    Task SetStateAsync(Region region);

    /// <summary>
    /// Register a person entity in this region
    /// </summary>
    Task RegisterPersonAsync(Guid personId);

    /// <summary>
    /// Register a house entity in this region
    /// </summary>
    Task RegisterHouseAsync(Guid houseId);

    /// <summary>
    /// Register a vehicle entity in this region
    /// </summary>
    Task RegisterVehicleAsync(Guid vehicleId);

    /// <summary>
    /// Register a business entity in this region
    /// </summary>
    Task RegisterBusinessAsync(Guid businessId);

    /// <summary>
    /// Get total energy consumption for this region
    /// </summary>
    Task<double> GetTotalEnergyAsync();

    /// <summary>
    /// Process a simulation tick (aggregate entity energy and publish to nation)
    /// </summary>
    Task TickAsync();
}
