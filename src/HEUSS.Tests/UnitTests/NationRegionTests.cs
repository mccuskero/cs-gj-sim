using HEUSS.Core.Enums;
using HEUSS.Core.Models;
using Xunit;

namespace HEUSS.Tests.UnitTests;

/// <summary>
/// Unit tests for Nation and Region models
/// </summary>
public class NationRegionTests
{
    [Fact]
    public void Nation_PerCapitaDailyEnergy_CalculatesCorrectly()
    {
        // Arrange
        var nation = new Nation
        {
            Name = "Test Nation",
            Population = 10_000_000, // 10 million
            TotalDailyEnergyConsumption = 500_000_000_000_000 // 500 TJ/day
        };

        // Act
        double perCapita = nation.PerCapitaDailyEnergy;

        // Assert
        // 500 TJ / 10M people = 50 MJ/person/day
        double expected = 500_000_000_000_000.0 / 10_000_000.0;
        Assert.Equal(expected, perCapita, precision: 0);
        Assert.Equal(50_000_000, perCapita, precision: 0); // 50 MJ
    }

    [Fact]
    public void Nation_ZeroPopulation_PerCapitaIsZero()
    {
        // Arrange
        var nation = new Nation
        {
            Name = "Empty Nation",
            Population = 0,
            TotalDailyEnergyConsumption = 1_000_000
        };

        // Act
        double perCapita = nation.PerCapitaDailyEnergy;

        // Assert
        Assert.Equal(0, perCapita);
    }

    [Fact]
    public void Nation_CanHaveMultipleRegions()
    {
        // Arrange
        var nation = new Nation
        {
            Name = "United States",
            CountryCode = "USA"
        };

        var california = Guid.NewGuid();
        var texas = Guid.NewGuid();
        var newYork = Guid.NewGuid();

        // Act
        nation.RegionIds.Add(california);
        nation.RegionIds.Add(texas);
        nation.RegionIds.Add(newYork);

        // Assert
        Assert.Equal(3, nation.RegionIds.Count);
        Assert.Contains(california, nation.RegionIds);
        Assert.Contains(texas, nation.RegionIds);
        Assert.Contains(newYork, nation.RegionIds);
    }

    [Fact]
    public void Region_PerCapitaDailyEnergy_CalculatesCorrectly()
    {
        // Arrange
        var region = new Region
        {
            Name = "California",
            Population = 40_000_000, // 40 million
            TotalDailyEnergyConsumption = 2_000_000_000_000_000 // 2 PJ/day (2000 TJ)
        };

        // Act
        double perCapita = region.PerCapitaDailyEnergy;

        // Assert
        // 2000 TJ / 40M = 50 MJ/person/day
        double expected = 2_000_000_000_000_000.0 / 40_000_000.0;
        Assert.Equal(expected, perCapita, precision: 0);
        Assert.Equal(50_000_000, perCapita, precision: 0);
    }

    [Fact]
    public void Region_InfrastructureAttributes_AreConfigurable()
    {
        // Arrange & Act
        var developedRegion = new Region
        {
            Name = "Silicon Valley",
            DevelopmentLevel = DevelopmentLevel.Developed,
            HasElectricity = true,
            ElectricityCoverage = 100.0,
            HasRunningWater = true,
            HasRoadNetwork = true
        };

        var subsistenceRegion = new Region
        {
            Name = "Rural Village",
            DevelopmentLevel = DevelopmentLevel.Subsistence,
            HasElectricity = false,
            ElectricityCoverage = 0.0,
            HasRunningWater = false,
            HasRoadNetwork = false
        };

        // Assert
        Assert.True(developedRegion.HasElectricity);
        Assert.Equal(100.0, developedRegion.ElectricityCoverage);
        Assert.True(developedRegion.HasRunningWater);

        Assert.False(subsistenceRegion.HasElectricity);
        Assert.Equal(0.0, subsistenceRegion.ElectricityCoverage);
        Assert.False(subsistenceRegion.HasRunningWater);
    }

    [Fact]
    public void Region_GeographicBounds_CanBeSet()
    {
        // Arrange
        var region = new Region
        {
            Name = "California",
            LatitudeNorth = 42.0,
            LatitudeSouth = 32.5,
            LongitudeEast = -114.1,
            LongitudeWest = -124.4
        };

        // Act & Assert
        Assert.Equal(42.0, region.LatitudeNorth);
        Assert.Equal(32.5, region.LatitudeSouth);
        Assert.Equal(-114.1, region.LongitudeEast);
        Assert.Equal(-124.4, region.LongitudeWest);
    }

    [Fact]
    public void Region_ClimateType_AffectsEnergyNeeds()
    {
        // Arrange
        var tropicalRegion = new Region
        {
            Name = "Miami",
            ClimateType = ClimateType.Tropical
        };

        var polarRegion = new Region
        {
            Name = "Alaska",
            ClimateType = ClimateType.Polar
        };

        // Act & Assert
        Assert.Equal(ClimateType.Tropical, tropicalRegion.ClimateType);
        Assert.Equal(ClimateType.Polar, polarRegion.ClimateType);
    }

    [Fact]
    public void Region_LastUpdated_IsTracked()
    {
        // Arrange
        var beforeCreation = DateTime.UtcNow.AddSeconds(-1);

        // Act
        var region = new Region
        {
            Name = "Test Region"
        };

        var afterCreation = DateTime.UtcNow.AddSeconds(1);

        // Assert
        Assert.True(region.LastUpdated > beforeCreation);
        Assert.True(region.LastUpdated < afterCreation);
    }

    [Fact]
    public void Region_PopulationDensity_IsTracked()
    {
        // Arrange
        var urbanRegion = new Region
        {
            Name = "Manhattan",
            PopulationDensity = 27_000 // 27k people per sq km
        };

        var ruralRegion = new Region
        {
            Name = "Wyoming",
            PopulationDensity = 2 // 2 people per sq km
        };

        // Act & Assert
        Assert.Equal(27_000, urbanRegion.PopulationDensity);
        Assert.Equal(2, ruralRegion.PopulationDensity);
        Assert.True(urbanRegion.PopulationDensity > ruralRegion.PopulationDensity);
    }
}
