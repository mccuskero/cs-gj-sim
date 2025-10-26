using HEUSS.Core.Utilities;
using Xunit;

namespace HEUSS.Tests.UnitTests;

/// <summary>
/// Unit tests for energy conversion utilities
/// </summary>
public class EnergyConversionTests
{
    [Fact]
    public void KilocaloriesToJoules_ConvertsCorrectly()
    {
        // Arrange
        double kcal = 2000; // 2000 kcal/day (typical diet)

        // Act
        double joules = EnergyConversion.KilocaloriesToJoules(kcal);

        // Assert
        Assert.Equal(8_368_000, joules); // 2000 * 4184
    }

    [Fact]
    public void KWhToJoules_ConvertsCorrectly()
    {
        // Arrange
        double kWh = 10; // 10 kWh

        // Act
        double joules = EnergyConversion.KWhToJoules(kWh);

        // Assert
        Assert.Equal(36_000_000, joules); // 10 * 3,600,000
    }

    [Fact]
    public void GallonsGasolineToJoules_ConvertsCorrectly()
    {
        // Arrange
        double gallons = 1;

        // Act
        double joules = EnergyConversion.GallonsGasolineToJoules(gallons);

        // Assert
        Assert.Equal(130_000_000, joules);
    }

    [Fact]
    public void JoulesToMegajoules_ConvertsCorrectly()
    {
        // Arrange
        double joules = 50_000_000; // 50 MJ

        // Act
        double mj = EnergyConversion.JoulesToMegajoules(joules);

        // Assert
        Assert.Equal(50, mj);
    }

    [Fact]
    public void FormatEnergy_FormatsGigajoulesCorrectly()
    {
        // Arrange
        double joules = 2_500_000_000; // 2.5 GJ

        // Act
        string formatted = EnergyConversion.FormatEnergy(joules);

        // Assert
        Assert.Equal("2.50 GJ", formatted);
    }

    [Fact]
    public void FormatEnergy_FormatsMegajoulesCorrectly()
    {
        // Arrange
        double joules = 150_000_000; // 150 MJ

        // Act
        string formatted = EnergyConversion.FormatEnergy(joules);

        // Assert
        Assert.Equal("150.00 MJ", formatted);
    }
}
