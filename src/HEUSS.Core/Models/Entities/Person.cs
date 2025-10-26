using HEUSS.Core.Enums;
using HEUSS.Core.Utilities;

namespace HEUSS.Core.Models.Entities;

/// <summary>
/// Represents an individual human in the simulation
/// Consumes biological energy from food; may also consume technological energy based on archetype
/// </summary>
public class Person : EnergyEntity
{
    #region Demographics

    /// <summary>
    /// Age in years
    /// </summary>
    public int Age { get; set; }

    /// <summary>
    /// Gender (M/F/O)
    /// </summary>
    public string Gender { get; set; } = "M";

    #endregion

    #region Socioeconomic Attributes

    /// <summary>
    /// Socioeconomic archetype determining energy consumption patterns
    /// </summary>
    public SocioeconomicArchetype SocioeconomicArchetype { get; set; }

    /// <summary>
    /// Does this person own a house? (0..n relationship)
    /// </summary>
    public bool OwnsHouse { get; set; }

    /// <summary>
    /// Does this person own a vehicle? (0..n relationship)
    /// </summary>
    public bool OwnsVehicle { get; set; }

    /// <summary>
    /// Does this person have access to electricity?
    /// Determined by region and individual circumstances
    /// </summary>
    public bool HasElectricityAccess { get; set; }

    #endregion

    #region Biological Energy

    /// <summary>
    /// Basal Metabolic Rate: energy required for basic bodily functions (joules/day)
    /// Typical range: 5,000,000 - 8,000,000 J/day (1,200-1,900 kcal/day)
    /// </summary>
    public double BasalMetabolicRate { get; set; } = 7_000_000; // ~1,675 kcal/day

    /// <summary>
    /// Activity factor multiplier (1.2 = sedentary, 1.5 = moderate, 2.0 = very active)
    /// </summary>
    public double ActivityFactor { get; set; } = 1.5;

    /// <summary>
    /// Daily food intake in joules
    /// Computed as BMR * ActivityFactor
    /// </summary>
    public double DailyFoodIntake => BasalMetabolicRate * ActivityFactor;

    #endregion

    #region Employment

    /// <summary>
    /// ID of business/organization where person works (optional)
    /// </summary>
    public Guid? EmployerId { get; set; }

    #endregion

    #region Ownership Relationships

    /// <summary>
    /// IDs of houses owned by this person (0..n)
    /// </summary>
    public List<Guid> OwnedHouseIds { get; set; } = new();

    /// <summary>
    /// IDs of vehicles owned by this person (0..n)
    /// </summary>
    public List<Guid> OwnedVehicleIds { get; set; } = new();

    #endregion

    /// <summary>
    /// Compute daily energy consumption (biological energy only for Person entity)
    /// Technological energy is attributed to House/Vehicle entities
    /// </summary>
    /// <returns>Biological energy consumption in joules/day</returns>
    public override double ComputeDailyEnergyUse()
    {
        // Person entity tracks only biological energy
        // Technological energy is computed by owned House and Vehicle entities
        return DailyFoodIntake;
    }

    /// <summary>
    /// Initialize BMR based on age and gender using Mifflin-St Jeor equation approximation
    /// </summary>
    public void InitializeBasalMetabolicRate(double weightKg = 70, double heightCm = 170)
    {
        // Mifflin-St Jeor equation:
        // Men: BMR = 10 * weight(kg) + 6.25 * height(cm) - 5 * age(y) + 5
        // Women: BMR = 10 * weight(kg) + 6.25 * height(cm) - 5 * age(y) - 161
        // Returns kcal/day, convert to joules

        double bmrKcal = Gender.ToUpper() == "M"
            ? 10 * weightKg + 6.25 * heightCm - 5 * Age + 5
            : 10 * weightKg + 6.25 * heightCm - 5 * Age - 161;

        BasalMetabolicRate = EnergyConversion.KilocaloriesToJoules(bmrKcal);
    }
}
