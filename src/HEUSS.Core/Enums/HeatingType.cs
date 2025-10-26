namespace HEUSS.Core.Enums;

/// <summary>
/// Types of heating systems for residential and commercial buildings
/// </summary>
public enum HeatingType
{
    /// <summary>
    /// No heating system
    /// </summary>
    None = 0,

    /// <summary>
    /// Electric resistance heating (baseboard, space heaters)
    /// </summary>
    Electric = 1,

    /// <summary>
    /// Natural gas furnace or boiler
    /// </summary>
    NaturalGas = 2,

    /// <summary>
    /// Heating oil furnace
    /// </summary>
    Oil = 3,

    /// <summary>
    /// Propane/LPG heating
    /// </summary>
    Propane = 4,

    /// <summary>
    /// Wood stove or fireplace
    /// </summary>
    Wood = 5,

    /// <summary>
    /// Ground-source heat pump (geothermal)
    /// </summary>
    Geothermal = 6,

    /// <summary>
    /// Air-source heat pump
    /// </summary>
    HeatPump = 7,

    /// <summary>
    /// District heating (centralized steam/hot water)
    /// </summary>
    DistrictHeating = 8
}
