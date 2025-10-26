namespace HEUSS.CLI.Models;

/// <summary>
/// Energy usage report for a geographic scope
/// </summary>
public class EnergyReport
{
    public string Name { get; set; } = string.Empty;
    public string CountryCode { get; set; } = string.Empty;
    public SimulationScope Scope { get; set; }
    public long Population { get; set; }
    public double TotalDailyEnergyJoules { get; set; }
    public double PerCapitaDailyEnergyJoules { get; set; }
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;

    // Entity breakdowns
    public EntityEnergyBreakdown Entities { get; set; } = new();

    // Regional data (for nation-level reports)
    public List<RegionalEnergyData> Regions { get; set; } = new();
}

/// <summary>
/// Energy consumption breakdown by entity type
/// </summary>
public class EntityEnergyBreakdown
{
    public long PersonCount { get; set; }
    public double PersonEnergyJoules { get; set; }

    public long HouseCount { get; set; }
    public double HouseEnergyJoules { get; set; }

    public long VehicleCount { get; set; }
    public double VehicleEnergyJoules { get; set; }

    public long BusinessCount { get; set; }
    public double BusinessEnergyJoules { get; set; }

    public long DataCenterCount { get; set; }
    public double DataCenterEnergyJoules { get; set; }

    public long FarmCount { get; set; }
    public double FarmEnergyJoules { get; set; }
    public double FarmFoodOutputJoules { get; set; }

    public double TotalBiologicalEnergyJoules => PersonEnergyJoules;
    public double TotalTechnologicalEnergyJoules =>
        HouseEnergyJoules + VehicleEnergyJoules + BusinessEnergyJoules +
        DataCenterEnergyJoules + FarmEnergyJoules;
}

/// <summary>
/// Regional energy data for nation-level aggregation
/// </summary>
public class RegionalEnergyData
{
    public string RegionName { get; set; } = string.Empty;
    public long Population { get; set; }
    public double TotalDailyEnergyJoules { get; set; }
    public double PerCapitaDailyEnergyJoules { get; set; }
}
