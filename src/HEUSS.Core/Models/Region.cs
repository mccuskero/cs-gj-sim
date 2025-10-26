using HEUSS.Core.Enums;

namespace HEUSS.Core.Models;

/// <summary>
/// Represents a geographic region (state, province, county)
/// Geographic container for entities; aggregates entity-level energy consumption
/// </summary>
public class Region
{
    /// <summary>
    /// Unique identifier
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Region name (e.g., "California", "Nairobi County")
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Parent nation
    /// </summary>
    public Guid NationId { get; set; }

    /// <summary>
    /// Climate classification
    /// </summary>
    public ClimateType ClimateType { get; set; }

    /// <summary>
    /// Population density (people per square kilometer)
    /// </summary>
    public double PopulationDensity { get; set; }

    /// <summary>
    /// Total population in this region
    /// </summary>
    public long Population { get; set; }

    /// <summary>
    /// Development level of the region
    /// </summary>
    public DevelopmentLevel DevelopmentLevel { get; set; }

    #region Infrastructure Attributes

    /// <summary>
    /// Does this region have electrical grid access?
    /// </summary>
    public bool HasElectricity { get; set; }

    /// <summary>
    /// Percentage of population with electricity access (0-100%)
    /// </summary>
    public double ElectricityCoverage { get; set; }

    /// <summary>
    /// Does this region have running water infrastructure?
    /// </summary>
    public bool HasRunningWater { get; set; }

    /// <summary>
    /// Are there paved roads for vehicles?
    /// </summary>
    public bool HasRoadNetwork { get; set; }

    #endregion

    #region Geographic Bounds

    /// <summary>
    /// Northern latitude boundary (decimal degrees)
    /// </summary>
    public double LatitudeNorth { get; set; }

    /// <summary>
    /// Southern latitude boundary (decimal degrees)
    /// </summary>
    public double LatitudeSouth { get; set; }

    /// <summary>
    /// Eastern longitude boundary (decimal degrees)
    /// </summary>
    public double LongitudeEast { get; set; }

    /// <summary>
    /// Western longitude boundary (decimal degrees)
    /// </summary>
    public double LongitudeWest { get; set; }

    #endregion

    #region Energy Aggregation

    /// <summary>
    /// Total daily energy consumption for this region (joules/day)
    /// Aggregated from all entities within the region
    /// </summary>
    public double TotalDailyEnergyConsumption { get; set; }

    /// <summary>
    /// Per capita daily energy consumption (joules/person/day)
    /// </summary>
    public double PerCapitaDailyEnergy => Population > 0
        ? TotalDailyEnergyConsumption / Population
        : 0;

    #endregion

    /// <summary>
    /// Timestamp of last energy aggregation update
    /// </summary>
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}
