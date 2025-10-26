using HEUSS.Core.Enums;
using HEUSS.Core.Models.Entities;
using HEUSS.Core.Utilities;
using Xunit;

namespace HEUSS.Tests.UnitTests;

/// <summary>
/// Unit tests for House entity
/// </summary>
public class HouseEntityTests
{
    [Fact]
    public void House_ComputeDailyEnergyUse_AggregatesAllComponents()
    {
        // Arrange
        var house = new House
        {
            Name = "Test House",
            SizeSquareMeters = 100,
            HouseholdCount = 3,
            DailyHvacEnergy = EnergyConversion.KWhToJoules(20),
            DailyApplianceEnergy = EnergyConversion.KWhToJoules(10),
            DailyLightingEnergy = EnergyConversion.KWhToJoules(3),
            DailyWaterHeatingEnergy = EnergyConversion.KWhToJoules(7)
        };

        // Act
        double totalEnergy = house.ComputeDailyEnergyUse();

        // Assert
        // Total should be 20 + 10 + 3 + 7 = 40 kWh = 144,000,000 J
        double expected = EnergyConversion.KWhToJoules(40);
        Assert.Equal(expected, totalEnergy);
    }

    [Fact]
    public void House_InitializeTypicalEnergyConsumption_TropicalClimate()
    {
        // Arrange
        var house = new House
        {
            SizeSquareMeters = 80,
            HouseholdCount = 4,
            InsulationFactor = 1.0
        };

        // Act
        house.InitializeTypicalEnergyConsumption(ClimateType.Tropical);

        // Assert
        // Tropical should have cooling-dominant energy use
        Assert.True(house.DailyHvacEnergy > 0);
        Assert.True(house.DailyApplianceEnergy > 0);
        Assert.True(house.DailyLightingEnergy > 0);
        Assert.True(house.DailyWaterHeatingEnergy > 0);

        // Verify total is reasonable (should be substantial for 80 sqm, 4 people)
        double total = house.ComputeDailyEnergyUse();
        Assert.True(total > EnergyConversion.KWhToJoules(20)); // At least 20 kWh/day
    }

    [Fact]
    public void House_InitializeTypicalEnergyConsumption_PolarClimate()
    {
        // Arrange
        var house = new House
        {
            SizeSquareMeters = 100,
            HouseholdCount = 3,
            InsulationFactor = 1.0
        };

        // Act
        house.InitializeTypicalEnergyConsumption(ClimateType.Polar);

        // Assert
        // Polar should have very high heating needs
        Assert.True(house.DailyHvacEnergy > 0);

        // Polar climate should consume more HVAC than tropical
        var tropicalHouse = new House
        {
            SizeSquareMeters = 100,
            HouseholdCount = 3,
            InsulationFactor = 1.0
        };
        tropicalHouse.InitializeTypicalEnergyConsumption(ClimateType.Tropical);

        Assert.True(house.DailyHvacEnergy > tropicalHouse.DailyHvacEnergy);
    }

    [Fact]
    public void House_InsulationFactor_AffectsHvacEnergy()
    {
        // Arrange
        var poorInsulation = new House
        {
            SizeSquareMeters = 100,
            HouseholdCount = 3,
            InsulationFactor = 1.5 // Poor insulation
        };

        var goodInsulation = new House
        {
            SizeSquareMeters = 100,
            HouseholdCount = 3,
            InsulationFactor = 0.7 // Good insulation
        };

        // Act
        poorInsulation.InitializeTypicalEnergyConsumption(ClimateType.Continental);
        goodInsulation.InitializeTypicalEnergyConsumption(ClimateType.Continental);

        // Assert
        // Poor insulation should consume more HVAC energy
        Assert.True(poorInsulation.DailyHvacEnergy > goodInsulation.DailyHvacEnergy);

        // Ratio should match insulation factors
        double expectedRatio = 1.5 / 0.7;
        double actualRatio = poorInsulation.DailyHvacEnergy / goodInsulation.DailyHvacEnergy;
        Assert.Equal(expectedRatio, actualRatio, precision: 1);
    }

    [Fact]
    public void House_MultipleOwners_CanBeTracked()
    {
        // Arrange
        var house = new House
        {
            Name = "Co-owned House"
        };

        var owner1 = Guid.NewGuid();
        var owner2 = Guid.NewGuid();

        // Act
        house.OwnerIds.Add(owner1);
        house.OwnerIds.Add(owner2);

        // Assert
        Assert.Equal(2, house.OwnerIds.Count);
        Assert.Contains(owner1, house.OwnerIds);
        Assert.Contains(owner2, house.OwnerIds);
    }

    [Fact]
    public void House_NoOwners_IsValid()
    {
        // Arrange & Act
        var house = new House
        {
            Name = "Rented House",
            SizeSquareMeters = 75,
            HouseholdCount = 2
        };

        // Assert
        Assert.Empty(house.OwnerIds);
        Assert.NotNull(house.OwnerIds);
    }

    [Fact]
    public void House_EnergyScalesWithHouseholdCount()
    {
        // Arrange
        var smallHousehold = new House
        {
            SizeSquareMeters = 80,
            HouseholdCount = 2,
            InsulationFactor = 1.0
        };

        var largeHousehold = new House
        {
            SizeSquareMeters = 80,
            HouseholdCount = 5,
            InsulationFactor = 1.0
        };

        // Act
        smallHousehold.InitializeTypicalEnergyConsumption(ClimateType.Temperate);
        largeHousehold.InitializeTypicalEnergyConsumption(ClimateType.Temperate);

        // Assert
        // Appliances, lighting, and water heating should scale with household count
        Assert.True(largeHousehold.DailyApplianceEnergy > smallHousehold.DailyApplianceEnergy);
        Assert.True(largeHousehold.DailyLightingEnergy > smallHousehold.DailyLightingEnergy);
        Assert.True(largeHousehold.DailyWaterHeatingEnergy > smallHousehold.DailyWaterHeatingEnergy);
    }
}
