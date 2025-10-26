namespace HEUSS.Core.Enums;

/// <summary>
/// Types of cooling systems for residential and commercial buildings
/// </summary>
public enum CoolingType
{
    /// <summary>
    /// No cooling system (natural ventilation only)
    /// </summary>
    None = 0,

    /// <summary>
    /// Central air conditioning
    /// </summary>
    CentralAC = 1,

    /// <summary>
    /// Window or wall-mounted air conditioning units
    /// </summary>
    WindowAC = 2,

    /// <summary>
    /// Portable air conditioning units
    /// </summary>
    PortableAC = 3,

    /// <summary>
    /// Evaporative cooler (swamp cooler)
    /// </summary>
    EvaporativeCooler = 4,

    /// <summary>
    /// Ceiling or floor fans only
    /// </summary>
    FansOnly = 5,

    /// <summary>
    /// Air-source heat pump (cooling mode)
    /// </summary>
    HeatPump = 6,

    /// <summary>
    /// Ground-source heat pump (geothermal cooling)
    /// </summary>
    Geothermal = 7,

    /// <summary>
    /// District cooling (centralized chilled water)
    /// </summary>
    DistrictCooling = 8,

    /// <summary>
    /// Electric resistance (rarely used for cooling)
    /// </summary>
    Electric = 9
}
