using HEUSS.Core.Enums;
using HEUSS.Core.Utilities;

namespace HEUSS.Core.Models.Entities;

/// <summary>
/// Represents a vehicle (car, truck, motorcycle) in the simulation
/// </summary>
public class Vehicle : EnergyEntity
{
    /// <summary>
    /// Owner of this vehicle (Person ID)
    /// </summary>
    public Guid OwnerId { get; set; }

    /// <summary>
    /// Type of fuel used
    /// </summary>
    public FuelType FuelType { get; set; }

    /// <summary>
    /// Fuel efficiency in kilometers per liter (for gasoline/diesel)
    /// </summary>
    public double KilometersPerLiter { get; set; }

    /// <summary>
    /// Energy efficiency in kWh per kilometer (for electric vehicles)
    /// </summary>
    public double KWhPerKilometer { get; set; }

    /// <summary>
    /// Average daily distance traveled in kilometers
    /// </summary>
    public double DailyDistanceKm { get; set; }

    /// <summary>
    /// Vehicle type (Car, Truck, Motorcycle)
    /// </summary>
    public string VehicleType { get; set; } = "Car";

    /// <summary>
    /// Compute daily energy consumption based on fuel type and distance
    /// </summary>
    public override double ComputeDailyEnergyUse()
    {
        return FuelType switch
        {
            FuelType.Gasoline => CalculateGasolineEnergy(),
            FuelType.Diesel => CalculateDieselEnergy(),
            FuelType.Electricity => CalculateElectricEnergy(),
            FuelType.Hybrid => CalculateHybridEnergy(),
            _ => 0
        };
    }

    private double CalculateGasolineEnergy()
    {
        if (KilometersPerLiter <= 0) return 0;
        double litersPerDay = DailyDistanceKm / KilometersPerLiter;
        return EnergyConversion.LitersGasolineToJoules(litersPerDay);
    }

    private double CalculateDieselEnergy()
    {
        if (KilometersPerLiter <= 0) return 0;
        double litersPerDay = DailyDistanceKm / KilometersPerLiter;
        return EnergyConversion.LitersDieselToJoules(litersPerDay);
    }

    private double CalculateElectricEnergy()
    {
        double kWhPerDay = KWhPerKilometer * DailyDistanceKm;
        return EnergyConversion.KWhToJoules(kWhPerDay);
    }

    private double CalculateHybridEnergy()
    {
        // Assume 60% electric, 40% gasoline for hybrid
        double electricPortion = CalculateElectricEnergy() * 0.6;
        double gasolinePortion = CalculateGasolineEnergy() * 0.4;
        return electricPortion + gasolinePortion;
    }
}
