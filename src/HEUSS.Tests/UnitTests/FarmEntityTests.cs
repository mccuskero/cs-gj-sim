using HEUSS.Core.Models.Entities;
using HEUSS.Core.Utilities;
using Xunit;

namespace HEUSS.Tests.UnitTests;

/// <summary>
/// Unit tests for Farm entity
/// </summary>
public class FarmEntityTests
{
    [Fact]
    public void Farm_ComputeDailyEnergyUse_CombinesFuelAndElectricity()
    {
        // Arrange
        var farm = new Farm
        {
            Name = "Test Farm",
            LandAreaHectares = 100,
            FarmType = "Crop",
            DailyFuelConsumptionLiters = 50,    // Diesel for tractors
            DailyElectricityKWh = 30             // Irrigation pumps
        };

        // Act
        double dailyEnergy = farm.ComputeDailyEnergyUse();

        // Assert
        // Fuel: 50 L * 38,600,000 J/L = 1,930,000,000 J
        // Electricity: 30 kWh * 3,600,000 J/kWh = 108,000,000 J
        // Total: 2,038,000,000 J
        double fuelEnergy = EnergyConversion.LitersDieselToJoules(50);
        double electricEnergy = EnergyConversion.KWhToJoules(30);
        double expected = fuelEnergy + electricEnergy;

        Assert.Equal(expected, dailyEnergy, precision: 0);
    }

    [Fact]
    public void Farm_ComputeNetFoodEnergyProduced_AccountsForWaste()
    {
        // Arrange
        var farm = new Farm
        {
            Name = "Wheat Farm",
            FoodOutputEnergyPerDay = EnergyConversion.KilocaloriesToJoules(1_000_000), // 1M kcal/day
            WasteRate = 0.3 // 30% waste
        };

        // Act
        double netFoodEnergy = farm.ComputeNetFoodEnergyProduced();

        // Assert
        // 1,000,000 kcal * (1 - 0.3) = 700,000 kcal
        double expected = EnergyConversion.KilocaloriesToJoules(700_000);
        Assert.Equal(expected, netFoodEnergy, precision: 0);
    }

    [Fact]
    public void Farm_EnergyReturnOnInvestment_CalculatesCorrectly()
    {
        // Arrange
        var farm = new Farm
        {
            Name = "Efficient Farm",
            DailyFuelConsumptionLiters = 10,
            DailyElectricityKWh = 5,
            FoodOutputEnergyPerDay = EnergyConversion.KilocaloriesToJoules(500_000), // 500k kcal
            WasteRate = 0.2 // 20% waste
        };

        // Act
        double eroi = farm.EnergyReturnOnInvestment;

        // Assert
        // Energy input: (10 L * 38.6 MJ) + (5 kWh * 3.6 MJ) = 386 MJ + 18 MJ = 404 MJ
        // Energy output: 500,000 kcal * 0.8 (waste) = 400,000 kcal = 1,673.6 MJ
        // EROI: 1673.6 / 404 ≈ 4.14
        double expectedEroi = farm.ComputeNetFoodEnergyProduced() / farm.ComputeDailyEnergyUse();

        Assert.Equal(expectedEroi, eroi, precision: 2);
        Assert.True(eroi > 1, "Farm should produce more energy than it consumes");
    }

    [Fact]
    public void Farm_HighWasteRate_ReducesNetOutput()
    {
        // Arrange
        var lowWaste = new Farm
        {
            FoodOutputEnergyPerDay = EnergyConversion.KilocaloriesToJoules(1_000_000),
            WasteRate = 0.1 // 10% waste
        };

        var highWaste = new Farm
        {
            FoodOutputEnergyPerDay = EnergyConversion.KilocaloriesToJoules(1_000_000),
            WasteRate = 0.4 // 40% waste
        };

        // Act
        double lowWasteOutput = lowWaste.ComputeNetFoodEnergyProduced();
        double highWasteOutput = highWaste.ComputeNetFoodEnergyProduced();

        // Assert
        // Low waste: 900,000 kcal
        // High waste: 600,000 kcal
        Assert.Equal(EnergyConversion.KilocaloriesToJoules(900_000), lowWasteOutput, precision: 0);
        Assert.Equal(EnergyConversion.KilocaloriesToJoules(600_000), highWasteOutput, precision: 0);
        Assert.True(lowWasteOutput > highWasteOutput);
    }

    [Fact]
    public void Farm_NoOperationalEnergy_ZeroConsumption()
    {
        // Arrange
        var farm = new Farm
        {
            Name = "Manual Labor Farm",
            DailyFuelConsumptionLiters = 0,
            DailyElectricityKWh = 0
        };

        // Act
        double dailyEnergy = farm.ComputeDailyEnergyUse();

        // Assert
        Assert.Equal(0, dailyEnergy);
    }

    [Fact]
    public void Farm_LivestockType_TracksCorrectly()
    {
        // Arrange
        var livestock = new Farm
        {
            Name = "Cattle Ranch",
            FarmType = "Livestock",
            LandAreaHectares = 500,
            DailyFuelConsumptionLiters = 20,
            DailyElectricityKWh = 10
        };

        // Act & Assert
        Assert.Equal("Livestock", livestock.FarmType);
        Assert.Equal(500, livestock.LandAreaHectares);
        Assert.True(livestock.ComputeDailyEnergyUse() > 0);
    }

    [Fact]
    public void Farm_MachineryCount_IsTracked()
    {
        // Arrange
        var farm = new Farm
        {
            Name = "Modern Farm",
            MachineryCount = 5,
            CropTypes = "Wheat, Corn, Soybeans"
        };

        // Act & Assert
        Assert.Equal(5, farm.MachineryCount);
        Assert.Equal("Wheat, Corn, Soybeans", farm.CropTypes);
    }
}
