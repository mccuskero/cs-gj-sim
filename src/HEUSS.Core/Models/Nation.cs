namespace HEUSS.Core.Models;

/// <summary>
/// Represents a nation/country in the simulation
/// Aggregates regional energy totals and supports longitudinal comparisons
/// </summary>
public class Nation
{
    /// <summary>
    /// Unique identifier
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Nation name (e.g., "United States", "Kenya")
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// ISO 3166-1 alpha-3 country code (e.g., "USA", "KEN")
    /// </summary>
    public string CountryCode { get; set; } = string.Empty;

    /// <summary>
    /// Total population
    /// </summary>
    public long Population { get; set; }

    /// <summary>
    /// GDP per capita in current US dollars
    /// </summary>
    public double GdpPerCapita { get; set; }

    /// <summary>
    /// List of regions within this nation
    /// </summary>
    public List<Guid> RegionIds { get; set; } = new();

    /// <summary>
    /// Total daily energy consumption across all regions (joules/day)
    /// Updated by aggregating regional data
    /// </summary>
    public double TotalDailyEnergyConsumption { get; set; }

    /// <summary>
    /// Per capita daily energy consumption (joules/person/day)
    /// </summary>
    public double PerCapitaDailyEnergy => Population > 0
        ? TotalDailyEnergyConsumption / Population
        : 0;
}
