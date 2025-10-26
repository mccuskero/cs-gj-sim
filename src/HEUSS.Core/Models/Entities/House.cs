using HEUSS.Core.Enums;
using HEUSS.Core.Utilities;

namespace HEUSS.Core.Models.Entities;

/// <summary>
/// Represents a residential building in the simulation
/// </summary>
public class House : EnergyEntity
{
    /// <summary>
    /// Owner(s) of this house (Person IDs)
    /// Can be owned by multiple people or none (rented)
    /// </summary>
    public List<Guid> OwnerIds { get; set; } = new();

    /// <summary>
    /// Size in square meters
    /// </summary>
    public double SizeSquareMeters { get; set; }

    /// <summary>
    /// Number of people living in the household
    /// </summary>
    public int HouseholdCount { get; set; }

    #region Heating and Cooling

    /// <summary>
    /// Type of heating fuel/energy source
    /// </summary>
    public FuelType HeatingType { get; set; }

    /// <summary>
    /// Type of cooling system fuel/energy source
    /// </summary>
    public FuelType CoolingType { get; set; } = FuelType.Electricity;

    /// <summary>
    /// Insulation quality factor (0.5 = poor, 1.0 = average, 1.5 = excellent)
    /// Lower value = less energy needed
    /// </summary>
    public double InsulationFactor { get; set; } = 1.0;

    #endregion

    #region Energy Consumption Components

    /// <summary>
    /// Daily HVAC (heating/cooling) energy consumption (joules/day)
    /// </summary>
    public double DailyHvacEnergy { get; set; }

    /// <summary>
    /// Daily appliance energy consumption (joules/day)
    /// </summary>
    public double DailyApplianceEnergy { get; set; }

    /// <summary>
    /// Daily lighting energy consumption (joules/day)
    /// </summary>
    public double DailyLightingEnergy { get; set; }

    /// <summary>
    /// Daily water heating energy consumption (joules/day)
    /// </summary>
    public double DailyWaterHeatingEnergy { get; set; }

    #endregion

    /// <summary>
    /// Compute total daily energy consumption for the house
    /// </summary>
    public override double ComputeDailyEnergyUse()
    {
        return DailyHvacEnergy + DailyApplianceEnergy + DailyLightingEnergy + DailyWaterHeatingEnergy;
    }

    /// <summary>
    /// Initialize typical energy consumption based on house size and household count
    /// </summary>
    public void InitializeTypicalEnergyConsumption(ClimateType climate)
    {
        // Base values per square meter per day
        double baseHvacPerSqM = climate switch
        {
            ClimateType.Tropical => EnergyConversion.KWhToJoules(0.15), // Cooling dominant
            ClimateType.Arid => EnergyConversion.KWhToJoules(0.20),
            ClimateType.Temperate => EnergyConversion.KWhToJoules(0.12),
            ClimateType.Continental => EnergyConversion.KWhToJoules(0.25), // Heating dominant
            ClimateType.Polar => EnergyConversion.KWhToJoules(0.35),
            ClimateType.Mediterranean => EnergyConversion.KWhToJoules(0.10),
            _ => EnergyConversion.KWhToJoules(0.15)
        };

        DailyHvacEnergy = baseHvacPerSqM * SizeSquareMeters * InsulationFactor;
        DailyApplianceEnergy = EnergyConversion.KWhToJoules(5.0 * HouseholdCount); // ~5 kWh/person/day
        DailyLightingEnergy = EnergyConversion.KWhToJoules(1.0 * HouseholdCount); // ~1 kWh/person/day
        DailyWaterHeatingEnergy = EnergyConversion.KWhToJoules(4.0 * HouseholdCount); // ~4 kWh/person/day
    }
}
