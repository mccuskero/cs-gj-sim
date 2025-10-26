using HEUSS.Core.Enums;
using HEUSS.Core.Models.Entities;
using HEUSS.Core.Utilities;
using Xunit;

namespace HEUSS.Tests.UnitTests;

/// <summary>
/// Unit tests for Person entity
/// </summary>
public class PersonEntityTests
{
    [Fact]
    public void Person_ComputeDailyEnergyUse_ReturnsCorrectBiologicalEnergy()
    {
        // Arrange
        var person = new Person
        {
            Name = "Test Person",
            Age = 30,
            Gender = "M",
            BasalMetabolicRate = 7_000_000, // ~1675 kcal/day
            ActivityFactor = 1.5
        };

        // Act
        double dailyEnergy = person.ComputeDailyEnergyUse();

        // Assert
        // Should be BMR * ActivityFactor
        Assert.Equal(10_500_000, dailyEnergy); // 7M * 1.5
    }

    [Fact]
    public void Person_InitializeBasalMetabolicRate_CalculatesCorrectlyForMale()
    {
        // Arrange
        var person = new Person
        {
            Age = 30,
            Gender = "M"
        };

        // Act
        person.InitializeBasalMetabolicRate(weightKg: 70, heightCm: 175);

        // Assert
        // Male BMR = 10 * 70 + 6.25 * 175 - 5 * 30 + 5 = 1648.75 kcal/day
        double expectedKcal = 1648.75;
        double expectedJoules = EnergyConversion.KilocaloriesToJoules(expectedKcal);
        Assert.Equal(expectedJoules, person.BasalMetabolicRate, precision: 0);
    }

    [Fact]
    public void Person_InitializeBasalMetabolicRate_CalculatesCorrectlyForFemale()
    {
        // Arrange
        var person = new Person
        {
            Age = 30,
            Gender = "F"
        };

        // Act
        person.InitializeBasalMetabolicRate(weightKg: 60, heightCm: 165);

        // Assert
        // Female BMR = 10 * 60 + 6.25 * 165 - 5 * 30 - 161 = 1321.25 kcal/day
        // Allow small tolerance due to floating point arithmetic
        double expectedKcal = 1321.25;
        double expectedJoules = EnergyConversion.KilocaloriesToJoules(expectedKcal);
        double tolerance = 5000; // 5000 J tolerance (~0.1% for typical BMR values)
        Assert.True(Math.Abs(expectedJoules - person.BasalMetabolicRate) < tolerance,
            $"Expected {expectedJoules:N0} ± {tolerance:N0}, but got {person.BasalMetabolicRate:N0}");
    }

    [Fact]
    public void Person_SubsistenceArchetype_HasZeroAssets()
    {
        // Arrange & Act
        var person = new Person
        {
            SocioeconomicArchetype = SocioeconomicArchetype.Subsistence,
            OwnsHouse = false,
            OwnsVehicle = false,
            HasElectricityAccess = false
        };

        // Assert
        Assert.Equal(SocioeconomicArchetype.Subsistence, person.SocioeconomicArchetype);
        Assert.False(person.OwnsHouse);
        Assert.False(person.OwnsVehicle);
        Assert.False(person.HasElectricityAccess);
        Assert.Empty(person.OwnedHouseIds);
        Assert.Empty(person.OwnedVehicleIds);
    }

    [Fact]
    public void Person_AffluentArchetype_HasMultipleAssets()
    {
        // Arrange & Act
        var person = new Person
        {
            SocioeconomicArchetype = SocioeconomicArchetype.Affluent,
            OwnsHouse = true,
            OwnsVehicle = true,
            HasElectricityAccess = true
        };

        // Simulate multiple assets
        person.OwnedHouseIds.Add(Guid.NewGuid());
        person.OwnedHouseIds.Add(Guid.NewGuid());
        person.OwnedVehicleIds.Add(Guid.NewGuid());
        person.OwnedVehicleIds.Add(Guid.NewGuid());
        person.OwnedVehicleIds.Add(Guid.NewGuid());

        // Assert
        Assert.Equal(SocioeconomicArchetype.Affluent, person.SocioeconomicArchetype);
        Assert.True(person.OwnsHouse);
        Assert.True(person.OwnsVehicle);
        Assert.True(person.HasElectricityAccess);
        Assert.Equal(2, person.OwnedHouseIds.Count);
        Assert.Equal(3, person.OwnedVehicleIds.Count);
    }
}
