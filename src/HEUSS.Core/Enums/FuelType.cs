namespace HEUSS.Core.Enums;

/// <summary>
/// Energy source types for vehicles and heating systems
/// </summary>
public enum FuelType
{
    /// <summary>
    /// Gasoline (petrol) for vehicles: ~34.2 MJ/liter
    /// </summary>
    Gasoline,

    /// <summary>
    /// Diesel fuel: ~38.6 MJ/liter
    /// </summary>
    Diesel,

    /// <summary>
    /// Electricity from grid or renewable sources: 3.6 MJ/kWh
    /// </summary>
    Electricity,

    /// <summary>
    /// Natural gas for heating and vehicles: ~38 MJ/cubic meter
    /// </summary>
    NaturalGas,

    /// <summary>
    /// Propane/LPG for heating and cooking
    /// </summary>
    Propane,

    /// <summary>
    /// Heating oil for residential heating
    /// </summary>
    HeatingOil,

    /// <summary>
    /// Hybrid: combination of electricity and gasoline/diesel
    /// </summary>
    Hybrid,

    /// <summary>
    /// Geothermal energy for heating/cooling
    /// </summary>
    Geothermal,

    /// <summary>
    /// Solar energy
    /// </summary>
    Solar,

    /// <summary>
    /// Wind energy
    /// </summary>
    Wind,

    /// <summary>
    /// Wood or biomass for heating/cooking
    /// </summary>
    Biomass
}
