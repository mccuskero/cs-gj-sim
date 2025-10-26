using HEUSS.Core.Enums;
using HEUSS.Core.Utilities;

namespace HEUSS.Core.Models.Entities;

/// <summary>
/// Represents a business or commercial organization
/// </summary>
public class Business : EnergyEntity
{
    /// <summary>
    /// Industry type (Office, Retail, Manufacturing)
    /// </summary>
    public string IndustryType { get; set; } = "Office";

    /// <summary>
    /// Number of employees
    /// </summary>
    public int Workforce { get; set; }

    /// <summary>
    /// Building size in square meters
    /// </summary>
    public double BuildingSizeSquareMeters { get; set; }

    /// <summary>
    /// Daily energy consumption per employee (joules)
    /// </summary>
    public double DailyEnergyPerEmployee { get; set; }

    /// <summary>
    /// Operational hours per day (affects energy usage)
    /// </summary>
    public int OperationalHoursPerDay { get; set; } = 8;

    /// <summary>
    /// Compute daily energy consumption based on workforce and industry type
    /// </summary>
    public override double ComputeDailyEnergyUse()
    {
        // Base energy per employee varies by industry
        double baseEnergyPerEmployee = IndustryType.ToLower() switch
        {
            "office" => EnergyConversion.KWhToJoules(20), // ~20 kWh/employee/day
            "retail" => EnergyConversion.KWhToJoules(15),
            "manufacturing" => EnergyConversion.KWhToJoules(100), // High energy use
            "warehouse" => EnergyConversion.KWhToJoules(25),
            _ => EnergyConversion.KWhToJoules(20)
        };

        // Adjust for operational hours (8 hours = 1.0, 24 hours = 3.0)
        double hoursFactor = OperationalHoursPerDay / 8.0;

        return baseEnergyPerEmployee * Workforce * hoursFactor;
    }
}
