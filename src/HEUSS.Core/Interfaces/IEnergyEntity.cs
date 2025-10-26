namespace HEUSS.Core.Interfaces;

/// <summary>
/// Interface for all entities that consume or produce energy in the simulation
/// </summary>
public interface IEnergyEntity
{
    /// <summary>
    /// Unique identifier for the entity
    /// </summary>
    Guid Id { get; set; }

    /// <summary>
    /// Human-readable name or description
    /// </summary>
    string Name { get; set; }

    /// <summary>
    /// Region where this entity is located
    /// </summary>
    Guid RegionId { get; set; }

    /// <summary>
    /// Compute the daily energy consumption or production in joules
    /// </summary>
    /// <returns>Energy in joules per day (positive = consumption, negative = production)</returns>
    double ComputeDailyEnergyUse();
}
