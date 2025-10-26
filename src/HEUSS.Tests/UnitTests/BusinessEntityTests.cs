using HEUSS.Core.Models.Entities;
using HEUSS.Core.Utilities;
using Xunit;

namespace HEUSS.Tests.UnitTests;

/// <summary>
/// Unit tests for Business entity
/// </summary>
public class BusinessEntityTests
{
    [Fact]
    public void Business_OfficeType_ComputesEnergyCorrectly()
    {
        // Arrange
        var business = new Business
        {
            Name = "Tech Office",
            IndustryType = "Office",
            Workforce = 100,
            OperationalHoursPerDay = 8
        };

        // Act
        double dailyEnergy = business.ComputeDailyEnergyUse();

        // Assert
        // Office: 20 kWh/employee/day * 100 employees * (8/8 hours factor)
        double expected = EnergyConversion.KWhToJoules(20 * 100 * 1.0);
        Assert.Equal(expected, dailyEnergy);
    }

    [Fact]
    public void Business_ManufacturingType_HighEnergyUse()
    {
        // Arrange
        var manufacturing = new Business
        {
            Name = "Factory",
            IndustryType = "Manufacturing",
            Workforce = 50,
            OperationalHoursPerDay = 16 // Two shifts
        };

        var office = new Business
        {
            Name = "Office",
            IndustryType = "Office",
            Workforce = 50,
            OperationalHoursPerDay = 8
        };

        // Act
        double mfgEnergy = manufacturing.ComputeDailyEnergyUse();
        double officeEnergy = office.ComputeDailyEnergyUse();

        // Assert
        // Manufacturing should use significantly more energy
        Assert.True(mfgEnergy > officeEnergy);

        // Manufacturing is 100 kWh/employee vs office 20 kWh/employee, plus 2x hours
        // So should be ~10x more energy
        Assert.True(mfgEnergy > officeEnergy * 5);
    }

    [Fact]
    public void Business_OperationalHours_ScalesEnergy()
    {
        // Arrange
        var standard = new Business
        {
            IndustryType = "Retail",
            Workforce = 30,
            OperationalHoursPerDay = 8
        };

        var extended = new Business
        {
            IndustryType = "Retail",
            Workforce = 30,
            OperationalHoursPerDay = 24 // 24/7 operation
        };

        // Act
        double standardEnergy = standard.ComputeDailyEnergyUse();
        double extendedEnergy = extended.ComputeDailyEnergyUse();

        // Assert
        // 24-hour operation should use 3x energy (24/8 = 3)
        double expectedRatio = 24.0 / 8.0;
        double actualRatio = extendedEnergy / standardEnergy;
        Assert.Equal(expectedRatio, actualRatio, precision: 1);
    }

    [Fact]
    public void Business_RetailType_ModerateEnergyUse()
    {
        // Arrange
        var retail = new Business
        {
            Name = "Store",
            IndustryType = "Retail",
            Workforce = 20,
            OperationalHoursPerDay = 10
        };

        // Act
        double dailyEnergy = retail.ComputeDailyEnergyUse();

        // Assert
        // Retail: 15 kWh/employee * 20 * (10/8) = 375 kWh
        double expectedKWh = 15 * 20 * (10.0 / 8.0);
        double expected = EnergyConversion.KWhToJoules(expectedKWh);
        Assert.Equal(expected, dailyEnergy, precision: 0);
    }

    [Fact]
    public void Business_ZeroWorkforce_ZeroEnergy()
    {
        // Arrange
        var business = new Business
        {
            Name = "Empty Office",
            IndustryType = "Office",
            Workforce = 0
        };

        // Act
        double dailyEnergy = business.ComputeDailyEnergyUse();

        // Assert
        Assert.Equal(0, dailyEnergy);
    }

    [Fact]
    public void Business_WarehouseType_UsesCorrectFormula()
    {
        // Arrange
        var warehouse = new Business
        {
            Name = "Distribution Center",
            IndustryType = "Warehouse",
            Workforce = 40,
            BuildingSizeSquareMeters = 5000,
            OperationalHoursPerDay = 12
        };

        // Act
        double dailyEnergy = warehouse.ComputeDailyEnergyUse();

        // Assert
        // Warehouse: 25 kWh/employee * 40 * (12/8) = 1,500 kWh
        double expectedKWh = 25 * 40 * (12.0 / 8.0);
        double expected = EnergyConversion.KWhToJoules(expectedKWh);
        Assert.Equal(expected, dailyEnergy, precision: 0);
    }
}
