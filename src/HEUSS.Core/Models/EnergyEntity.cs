using HEUSS.Core.Interfaces;

namespace HEUSS.Core.Models;

/// <summary>
/// Abstract base class for all energy-consuming or producing entities
/// </summary>
public abstract class EnergyEntity : IEnergyEntity
{
    /// <summary>
    /// Unique identifier for the entity
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Human-readable name or description
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Region where this entity is located
    /// </summary>
    public Guid RegionId { get; set; }

    /// <summary>
    /// Compute the daily energy consumption or production in joules
    /// Must be implemented by derived classes
    /// </summary>
    /// <returns>Energy in joules per day (positive = consumption, negative = production)</returns>
    public abstract double ComputeDailyEnergyUse();
}
