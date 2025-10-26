using HEUSS.Core.Enums;
using HEUSS.Core.Models.Entities;
using HEUSS.Core.Utilities;
using Xunit;

namespace HEUSS.Tests.UnitTests;

/// <summary>
/// Unit tests for Vehicle entity
/// </summary>
public class VehicleEntityTests
{
    [Fact]
    public void Vehicle_GasolineCar_ComputesEnergyCorrectly()
    {
        // Arrange
        var vehicle = new Vehicle
        {
            Name = "Gasoline Car",
            FuelType = FuelType.Gasoline,
            KilometersPerLiter = 12, // ~12 km/L efficiency
            DailyDistanceKm = 30     // 30 km/day commute
        };

        // Act
        double dailyEnergy = vehicle.ComputeDailyEnergyUse();

        // Assert
        // 30 km / 12 km/L = 2.5 liters/day
        // 2.5 L * 34,200,000 J/L = 85,500,000 J
        double expected = 2.5 * EnergyConversion.JOULES_PER_LITER_GASOLINE;
        Assert.Equal(expected, dailyEnergy, precision: 0);
    }

    [Fact]
    public void Vehicle_ElectricCar_ComputesEnergyCorrectly()
    {
        // Arrange
        var vehicle = new Vehicle
        {
            Name = "Electric Car",
            FuelType = FuelType.Electricity,
            KWhPerKilometer = 0.15, // 0.15 kWh/km efficiency
            DailyDistanceKm = 40     // 40 km/day
        };

        // Act
        double dailyEnergy = vehicle.ComputeDailyEnergyUse();

        // Assert
        // 40 km * 0.15 kWh/km = 6 kWh/day
        // 6 kWh * 3,600,000 J/kWh = 21,600,000 J
        double expectedKWh = 6;
        double expected = EnergyConversion.KWhToJoules(expectedKWh);
        Assert.Equal(expected, dailyEnergy, precision: 0);
    }

    [Fact]
    public void Vehicle_DieselTruck_ComputesEnergyCorrectly()
    {
        // Arrange
        var vehicle = new Vehicle
        {
            Name = "Diesel Truck",
            FuelType = FuelType.Diesel,
            VehicleType = "Truck",
            KilometersPerLiter = 8,  // Lower efficiency
            DailyDistanceKm = 100    // Long-haul
        };

        // Act
        double dailyEnergy = vehicle.ComputeDailyEnergyUse();

        // Assert
        // 100 km / 8 km/L = 12.5 liters/day
        // 12.5 L * 38,600,000 J/L = 482,500,000 J
        double expected = 12.5 * EnergyConversion.JOULES_PER_LITER_DIESEL;
        Assert.Equal(expected, dailyEnergy, precision: 0);
    }

    [Fact]
    public void Vehicle_HybridCar_ComputesEnergyCorrectly()
    {
        // Arrange
        var vehicle = new Vehicle
        {
            Name = "Hybrid Car",
            FuelType = FuelType.Hybrid,
            KilometersPerLiter = 18,    // Higher efficiency
            KWhPerKilometer = 0.08,     // Electric portion
            DailyDistanceKm = 50
        };

        // Act
        double dailyEnergy = vehicle.ComputeDailyEnergyUse();

        // Assert
        // Hybrid: 60% electric, 40% gasoline
        // Electric: 50 km * 0.08 kWh/km * 0.6 = 2.4 kWh
        // Gasoline: (50 km / 18 km/L) * 0.4 = 1.11 liters
        double electricPortion = EnergyConversion.KWhToJoules(50 * 0.08) * 0.6;
        double gasolinePortion = EnergyConversion.LitersGasolineToJoules(50.0 / 18) * 0.4;
        double expected = electricPortion + gasolinePortion;

        Assert.Equal(expected, dailyEnergy, precision: 2); // Allow small rounding differences
    }
}
