using HEUSS.Core.Utilities;

namespace HEUSS.Core.Models.Entities;

/// <summary>
/// Represents a data center facility
/// Specialized type of business with very high energy consumption
/// </summary>
public class DataCenter : EnergyEntity
{
    /// <summary>
    /// Data center size category
    /// </summary>
    public string SizeCategory { get; set; } = "Small"; // Small, Large, CloudProvider, LLMTraining

    /// <summary>
    /// Number of servers
    /// </summary>
    public int ServerCount { get; set; }

    /// <summary>
    /// Power Usage Effectiveness (PUE) ratio
    /// Total facility power / IT equipment power
    /// Ideal: 1.0, Excellent: 1.2, Average: 1.5-1.8, Poor: 2.0+
    /// </summary>
    public double PowerUsageEffectiveness { get; set; } = 1.5;

    /// <summary>
    /// Average power per server in watts
    /// </summary>
    public double AveragePowerPerServer { get; set; } = 500; // 500W typical

    /// <summary>
    /// Workload type (Compute, Storage, ML_Training, Web_Hosting)
    /// </summary>
    public string WorkloadType { get; set; } = "Compute";

    /// <summary>
    /// Cooling system type
    /// </summary>
    public string CoolingSystem { get; set; } = "AirCooled"; // AirCooled, WaterCooled, Geothermal

    /// <summary>
    /// Compute daily energy consumption
    /// Data centers operate 24/7
    /// </summary>
    public override double ComputeDailyEnergyUse()
    {
        // IT equipment power (watts) = ServerCount * AveragePowerPerServer
        double itPowerWatts = ServerCount * AveragePowerPerServer;

        // Total facility power including cooling (apply PUE)
        double totalFacilityPowerWatts = itPowerWatts * PowerUsageEffectiveness;

        // Convert watts to joules/day (watts * seconds in a day)
        // 1 watt = 1 joule/second
        // 1 day = 86,400 seconds
        double joulesPerDay = totalFacilityPowerWatts * 86_400;

        return joulesPerDay;
    }

    /// <summary>
    /// Initialize data center based on size category
    /// </summary>
    public void InitializeBySize(string size)
    {
        SizeCategory = size;

        switch (size.ToLower())
        {
            case "small":
                ServerCount = 100;
                PowerUsageEffectiveness = 1.8;
                AveragePowerPerServer = 400;
                break;

            case "large":
                ServerCount = 5000;
                PowerUsageEffectiveness = 1.4;
                AveragePowerPerServer = 500;
                break;

            case "cloudprovider":
                ServerCount = 50000;
                PowerUsageEffectiveness = 1.2;
                AveragePowerPerServer = 600;
                break;

            case "llmtraining":
                ServerCount = 10000;
                PowerUsageEffectiveness = 1.3;
                AveragePowerPerServer = 1500; // High-power GPUs
                break;
        }
    }
}
