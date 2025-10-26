using HEUSS.CLI.Models;
using HEUSS.Core.Enums;
using HEUSS.Core.Models;
using HEUSS.Core.Models.Entities;
using Serilog;

namespace HEUSS.CLI.Services;

/// <summary>
/// Runs energy simulations and generates reports
/// NOTE: This is a mock implementation for Phase 1. In Phase 2, this will connect to Orleans grains.
/// </summary>
public class SimulationRunner : ISimulationRunner
{
    private readonly ILogger _logger;

    public SimulationRunner(ILogger logger)
    {
        _logger = logger;
    }

    public async Task<List<EnergyReport>> RunGlobalAsync(CancellationToken cancellationToken = default)
    {
        _logger.Information("Running global simulation for all nations...");

        // Mock data - In Phase 2, this will query all NationGrains
        var reports = new List<EnergyReport>
        {
            await GenerateMockNationReport("USA", "United States", 331_000_000, SocioeconomicArchetype.UpperMiddle),
            await GenerateMockNationReport("KEN", "Kenya", 54_000_000, SocioeconomicArchetype.LowIncome),
            await GenerateMockNationReport("IND", "India", 1_400_000_000, SocioeconomicArchetype.LowerMiddle),
            await GenerateMockNationReport("JPN", "Japan", 125_000_000, SocioeconomicArchetype.UpperMiddle),
            await GenerateMockNationReport("BRA", "Brazil", 215_000_000, SocioeconomicArchetype.Middle)
        };

        _logger.Information("Global simulation completed. {Count} nations processed.", reports.Count);
        return reports;
    }

    public async Task<EnergyReport> RunNationAsync(string countryCode, CancellationToken cancellationToken = default)
    {
        _logger.Information("Running simulation for nation: {CountryCode}", countryCode);

        // Mock data - In Phase 2, this will query the specific NationGrain
        var report = countryCode.ToUpper() switch
        {
            "USA" => await GenerateMockNationReport("USA", "United States", 331_000_000, SocioeconomicArchetype.UpperMiddle, includeRegions: true),
            "KEN" => await GenerateMockNationReport("KEN", "Kenya", 54_000_000, SocioeconomicArchetype.LowIncome, includeRegions: true),
            "IND" => await GenerateMockNationReport("IND", "India", 1_400_000_000, SocioeconomicArchetype.LowerMiddle, includeRegions: true),
            _ => throw new ArgumentException($"Nation '{countryCode}' not found in simulation data.")
        };

        _logger.Information("Nation simulation completed for {Name}", report.Name);
        return report;
    }

    public async Task<EnergyReport> RunRegionAsync(string countryCode, string regionName, CancellationToken cancellationToken = default)
    {
        _logger.Information("Running simulation for region: {Region}, {Country}", regionName, countryCode);

        // Mock data - In Phase 2, this will query the specific RegionGrain
        var report = await GenerateMockRegionReport(countryCode, regionName, 40_000_000, SocioeconomicArchetype.UpperMiddle);

        _logger.Information("Region simulation completed for {Name}", report.Name);
        return report;
    }

    #region Mock Data Generation (Remove in Phase 2)

    private async Task<EnergyReport> GenerateMockNationReport(
        string countryCode,
        string name,
        long population,
        SocioeconomicArchetype dominantArchetype,
        bool includeRegions = false)
    {
        await Task.Delay(100); // Simulate async work

        var perCapitaEnergy = GetArchetypeEnergyRange(dominantArchetype).Average;
        var totalEnergy = perCapitaEnergy * population;

        var report = new EnergyReport
        {
            Name = name,
            CountryCode = countryCode,
            Scope = SimulationScope.Nation,
            Population = population,
            TotalDailyEnergyJoules = totalEnergy,
            PerCapitaDailyEnergyJoules = perCapitaEnergy,
            Entities = GenerateMockEntityBreakdown(population, dominantArchetype)
        };

        if (includeRegions)
        {
            report.Regions = GenerateMockRegions(population, dominantArchetype);
        }

        return report;
    }

    private async Task<EnergyReport> GenerateMockRegionReport(
        string countryCode,
        string regionName,
        long population,
        SocioeconomicArchetype dominantArchetype)
    {
        await Task.Delay(50); // Simulate async work

        var perCapitaEnergy = GetArchetypeEnergyRange(dominantArchetype).Average;
        var totalEnergy = perCapitaEnergy * population;

        return new EnergyReport
        {
            Name = regionName,
            CountryCode = countryCode,
            Scope = SimulationScope.Region,
            Population = population,
            TotalDailyEnergyJoules = totalEnergy,
            PerCapitaDailyEnergyJoules = perCapitaEnergy,
            Entities = GenerateMockEntityBreakdown(population, dominantArchetype)
        };
    }

    private EntityEnergyBreakdown GenerateMockEntityBreakdown(long population, SocioeconomicArchetype archetype)
    {
        var energyRange = GetArchetypeEnergyRange(archetype);
        var biologicalPercent = GetArchetypeBiologicalPercent(archetype);

        var personEnergy = population * 10_000_000; // ~10 MJ/person biological
        var totalEnergy = population * energyRange.Average;

        return new EntityEnergyBreakdown
        {
            PersonCount = population,
            PersonEnergyJoules = personEnergy,
            HouseCount = (long)(population * GetHouseOwnershipRate(archetype)),
            HouseEnergyJoules = totalEnergy * 0.4,
            VehicleCount = (long)(population * GetVehicleOwnershipRate(archetype)),
            VehicleEnergyJoules = totalEnergy * 0.3,
            BusinessCount = population / 50,
            BusinessEnergyJoules = totalEnergy * 0.2,
            DataCenterCount = population / 100_000,
            DataCenterEnergyJoules = totalEnergy * 0.05,
            FarmCount = population / 1000,
            FarmEnergyJoules = totalEnergy * 0.03,
            FarmFoodOutputJoules = totalEnergy * 0.15
        };
    }

    private List<RegionalEnergyData> GenerateMockRegions(long population, SocioeconomicArchetype archetype)
    {
        var perCapita = GetArchetypeEnergyRange(archetype).Average;

        return new List<RegionalEnergyData>
        {
            new() { RegionName = "Region A", Population = population / 3, TotalDailyEnergyJoules = (population / 3) * perCapita, PerCapitaDailyEnergyJoules = perCapita },
            new() { RegionName = "Region B", Population = population / 3, TotalDailyEnergyJoules = (population / 3) * perCapita * 1.2, PerCapitaDailyEnergyJoules = perCapita * 1.2 },
            new() { RegionName = "Region C", Population = population / 3, TotalDailyEnergyJoules = (population / 3) * perCapita * 0.8, PerCapitaDailyEnergyJoules = perCapita * 0.8 }
        };
    }

    private static (double Min, double Max, double Average) GetArchetypeEnergyRange(SocioeconomicArchetype archetype)
    {
        return archetype switch
        {
            SocioeconomicArchetype.Subsistence => (6_000_000, 12_000_000, 9_000_000),
            SocioeconomicArchetype.LowIncome => (12_000_000, 30_000_000, 21_000_000),
            SocioeconomicArchetype.LowerMiddle => (30_000_000, 80_000_000, 55_000_000),
            SocioeconomicArchetype.Middle => (80_000_000, 200_000_000, 140_000_000),
            SocioeconomicArchetype.UpperMiddle => (200_000_000, 400_000_000, 300_000_000),
            SocioeconomicArchetype.Affluent => (400_000_000, 1_000_000_000, 700_000_000),
            _ => (50_000_000, 200_000_000, 125_000_000)
        };
    }

    private static double GetArchetypeBiologicalPercent(SocioeconomicArchetype archetype)
    {
        return archetype switch
        {
            SocioeconomicArchetype.Subsistence => 100.0,
            SocioeconomicArchetype.LowIncome => 80.0,
            SocioeconomicArchetype.LowerMiddle => 40.0,
            SocioeconomicArchetype.Middle => 20.0,
            SocioeconomicArchetype.UpperMiddle => 10.0,
            SocioeconomicArchetype.Affluent => 5.0,
            _ => 50.0
        };
    }

    private static double GetHouseOwnershipRate(SocioeconomicArchetype archetype)
    {
        return archetype switch
        {
            SocioeconomicArchetype.Subsistence => 0.0,
            SocioeconomicArchetype.LowIncome => 0.1,
            SocioeconomicArchetype.LowerMiddle => 0.3,
            SocioeconomicArchetype.Middle => 0.6,
            SocioeconomicArchetype.UpperMiddle => 0.9,
            SocioeconomicArchetype.Affluent => 1.2,
            _ => 0.5
        };
    }

    private static double GetVehicleOwnershipRate(SocioeconomicArchetype archetype)
    {
        return archetype switch
        {
            SocioeconomicArchetype.Subsistence => 0.0,
            SocioeconomicArchetype.LowIncome => 0.02,
            SocioeconomicArchetype.LowerMiddle => 0.2,
            SocioeconomicArchetype.Middle => 0.8,
            SocioeconomicArchetype.UpperMiddle => 1.5,
            SocioeconomicArchetype.Affluent => 2.5,
            _ => 0.7
        };
    }

    #endregion
}
