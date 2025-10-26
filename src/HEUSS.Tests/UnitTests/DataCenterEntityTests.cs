using HEUSS.Core.Models.Entities;
using Xunit;

namespace HEUSS.Tests.UnitTests;

/// <summary>
/// Unit tests for DataCenter entity
/// </summary>
public class DataCenterEntityTests
{
    [Fact]
    public void DataCenter_SmallSize_ComputesEnergyCorrectly()
    {
        // Arrange
        var dataCenter = new DataCenter();
        dataCenter.InitializeBySize("small");

        // Act
        double dailyEnergy = dataCenter.ComputeDailyEnergyUse();

        // Assert
        // Small: 100 servers * 400W * 1.8 PUE = 72,000W total
        // 72,000W * 86,400 seconds/day = 6,220,800,000 J/day (~6.2 GJ/day)
        double expectedWatts = 100 * 400 * 1.8;
        double expectedJoules = expectedWatts * 86_400;
        Assert.Equal(expectedJoules, dailyEnergy, precision: 0);
    }

    [Fact]
    public void DataCenter_CloudProvider_ComputesEnergyCorrectly()
    {
        // Arrange
        var dataCenter = new DataCenter();
        dataCenter.InitializeBySize("cloudprovider");

        // Act
        double dailyEnergy = dataCenter.ComputeDailyEnergyUse();

        // Assert
        // CloudProvider: 50,000 servers * 600W * 1.2 PUE = 36,000,000W
        // 36,000,000W * 86,400 seconds/day = 3,110,400,000,000 J/day (~3.1 TJ/day)
        double expectedWatts = 50_000 * 600 * 1.2;
        double expectedJoules = expectedWatts * 86_400;
        Assert.Equal(expectedJoules, dailyEnergy, precision: 0);
    }

    [Fact]
    public void DataCenter_LLMTraining_HasHighPowerPerServer()
    {
        // Arrange
        var dataCenter = new DataCenter();
        dataCenter.InitializeBySize("llmtraining");

        // Assert
        Assert.Equal(1500, dataCenter.AveragePowerPerServer); // High-power GPUs
        Assert.Equal(10_000, dataCenter.ServerCount);
        Assert.Equal(1.3, dataCenter.PowerUsageEffectiveness);
    }

    [Fact]
    public void DataCenter_PUE_AffectsEnergyConsumption()
    {
        // Arrange
        var efficientDC = new DataCenter
        {
            ServerCount = 1000,
            AveragePowerPerServer = 500,
            PowerUsageEffectiveness = 1.2 // Efficient
        };

        var inefficientDC = new DataCenter
        {
            ServerCount = 1000,
            AveragePowerPerServer = 500,
            PowerUsageEffectiveness = 2.0 // Inefficient
        };

        // Act
        double efficientEnergy = efficientDC.ComputeDailyEnergyUse();
        double inefficientEnergy = inefficientDC.ComputeDailyEnergyUse();

        // Assert
        // Inefficient should consume ~1.67x more (2.0 / 1.2)
        double ratio = inefficientEnergy / efficientEnergy;
        Assert.Equal(2.0 / 1.2, ratio, precision: 2);
    }
}
