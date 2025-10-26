using HEUSS.Core.Utilities;

namespace HEUSS.Core.Models.Entities;

/// <summary>
/// Represents a farm or food production facility
/// Both consumes energy (operations) and produces energy (food calories)
/// </summary>
public class Farm : EnergyEntity
{
    /// <summary>
    /// Land area in hectares
    /// </summary>
    public double LandAreaHectares { get; set; }

    /// <summary>
    /// Type of farm (Crop, Livestock, Mixed, Aquaculture)
    /// </summary>
    public string FarmType { get; set; } = "Crop";

    /// <summary>
    /// Primary crops grown (comma-separated)
    /// </summary>
    public string CropTypes { get; set; } = string.Empty;

    /// <summary>
    /// Daily food energy output in joules
    /// </summary>
    public double FoodOutputEnergyPerDay { get; set; }

    /// <summary>
    /// Waste rate (0.0 - 1.0)
    /// Percentage of food energy lost in production/distribution
    /// </summary>
    public double WasteRate { get; set; } = 0.25; // 25% typical

    /// <summary>
    /// Number of machinery/equipment (tractors, harvesters, etc.)
    /// </summary>
    public int MachineryCount { get; set; }

    /// <summary>
    /// Daily diesel/fuel consumption for machinery (liters)
    /// </summary>
    public double DailyFuelConsumptionLiters { get; set; }

    /// <summary>
    /// Daily electricity consumption for irrigation, processing, storage (kWh)
    /// </summary>
    public double DailyElectricityKWh { get; set; }

    /// <summary>
    /// Compute daily energy consumption (input energy for operations)
    /// This represents energy consumed, not food energy produced
    /// </summary>
    public override double ComputeDailyEnergyUse()
    {
        // Energy consumed for operations
        double fuelEnergy = EnergyConversion.LitersDieselToJoules(DailyFuelConsumptionLiters);
        double electricEnergy = EnergyConversion.KWhToJoules(DailyElectricityKWh);

        return fuelEnergy + electricEnergy;
    }

    /// <summary>
    /// Net food energy produced (output - waste)
    /// This is separate from operational energy consumption
    /// </summary>
    /// <returns>Net food energy in joules/day</returns>
    public double ComputeNetFoodEnergyProduced()
    {
        return FoodOutputEnergyPerDay * (1 - WasteRate);
    }

    /// <summary>
    /// Energy Return on Investment (EROI)
    /// Ratio of food energy produced to operational energy consumed
    /// </summary>
    public double EnergyReturnOnInvestment
    {
        get
        {
            double operationalEnergy = ComputeDailyEnergyUse();
            if (operationalEnergy == 0) return 0;
            return ComputeNetFoodEnergyProduced() / operationalEnergy;
        }
    }
}
