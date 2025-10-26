namespace HEUSS.Core.Utilities;

/// <summary>
/// Utility class for converting between different energy units
/// All conversions are to/from joules (the base SI unit)
/// </summary>
public static class EnergyConversion
{
    #region Constants

    /// <summary>
    /// 1 kilocalorie (food "Calorie") = 4,184 joules
    /// </summary>
    public const double JOULES_PER_KILOCALORIE = 4184.0;

    /// <summary>
    /// 1 kilowatt-hour = 3,600,000 joules (3.6 MJ)
    /// </summary>
    public const double JOULES_PER_KWH = 3_600_000.0;

    /// <summary>
    /// 1 gallon of gasoline = 130,000,000 joules (130 MJ)
    /// </summary>
    public const double JOULES_PER_GALLON_GASOLINE = 130_000_000.0;

    /// <summary>
    /// 1 liter of gasoline = 34,200,000 joules (34.2 MJ)
    /// </summary>
    public const double JOULES_PER_LITER_GASOLINE = 34_200_000.0;

    /// <summary>
    /// 1 liter of diesel = 38,600,000 joules (38.6 MJ)
    /// </summary>
    public const double JOULES_PER_LITER_DIESEL = 38_600_000.0;

    /// <summary>
    /// 1 cubic meter of natural gas = 38,000,000 joules (38 MJ)
    /// </summary>
    public const double JOULES_PER_CUBIC_METER_NATURAL_GAS = 38_000_000.0;

    /// <summary>
    /// 1 barrel of oil = 6,000,000,000 joules (6 GJ)
    /// </summary>
    public const double JOULES_PER_BARREL_OIL = 6_000_000_000.0;

    /// <summary>
    /// 1 megajoule = 1,000,000 joules
    /// </summary>
    public const double JOULES_PER_MEGAJOULE = 1_000_000.0;

    /// <summary>
    /// 1 gigajoule = 1,000,000,000 joules
    /// </summary>
    public const double JOULES_PER_GIGAJOULE = 1_000_000_000.0;

    #endregion

    #region Food/Caloric Energy

    /// <summary>
    /// Convert kilocalories (food Calories) to joules
    /// </summary>
    /// <param name="kilocalories">Energy in kilocalories</param>
    /// <returns>Energy in joules</returns>
    public static double KilocaloriesToJoules(double kilocalories)
        => kilocalories * JOULES_PER_KILOCALORIE;

    /// <summary>
    /// Convert joules to kilocalories (food Calories)
    /// </summary>
    /// <param name="joules">Energy in joules</param>
    /// <returns>Energy in kilocalories</returns>
    public static double JoulesToKilocalories(double joules)
        => joules / JOULES_PER_KILOCALORIE;

    #endregion

    #region Electrical Energy

    /// <summary>
    /// Convert kilowatt-hours to joules
    /// </summary>
    /// <param name="kWh">Energy in kilowatt-hours</param>
    /// <returns>Energy in joules</returns>
    public static double KWhToJoules(double kWh)
        => kWh * JOULES_PER_KWH;

    /// <summary>
    /// Convert joules to kilowatt-hours
    /// </summary>
    /// <param name="joules">Energy in joules</param>
    /// <returns>Energy in kilowatt-hours</returns>
    public static double JoulesToKWh(double joules)
        => joules / JOULES_PER_KWH;

    #endregion

    #region Vehicle Fuel

    /// <summary>
    /// Convert gallons of gasoline to joules
    /// </summary>
    /// <param name="gallons">Volume in gallons</param>
    /// <returns>Energy in joules</returns>
    public static double GallonsGasolineToJoules(double gallons)
        => gallons * JOULES_PER_GALLON_GASOLINE;

    /// <summary>
    /// Convert liters of gasoline to joules
    /// </summary>
    /// <param name="liters">Volume in liters</param>
    /// <returns>Energy in joules</returns>
    public static double LitersGasolineToJoules(double liters)
        => liters * JOULES_PER_LITER_GASOLINE;

    /// <summary>
    /// Convert liters of diesel to joules
    /// </summary>
    /// <param name="liters">Volume in liters</param>
    /// <returns>Energy in joules</returns>
    public static double LitersDieselToJoules(double liters)
        => liters * JOULES_PER_LITER_DIESEL;

    /// <summary>
    /// Convert cubic meters of natural gas to joules
    /// </summary>
    /// <param name="cubicMeters">Volume in cubic meters</param>
    /// <returns>Energy in joules</returns>
    public static double CubicMetersNaturalGasToJoules(double cubicMeters)
        => cubicMeters * JOULES_PER_CUBIC_METER_NATURAL_GAS;

    #endregion

    #region Large-Scale Units

    /// <summary>
    /// Convert megajoules to joules
    /// </summary>
    /// <param name="megajoules">Energy in megajoules</param>
    /// <returns>Energy in joules</returns>
    public static double MegajoulesToJoules(double megajoules)
        => megajoules * JOULES_PER_MEGAJOULE;

    /// <summary>
    /// Convert joules to megajoules
    /// </summary>
    /// <param name="joules">Energy in joules</param>
    /// <returns>Energy in megajoules</returns>
    public static double JoulesToMegajoules(double joules)
        => joules / JOULES_PER_MEGAJOULE;

    /// <summary>
    /// Convert gigajoules to joules
    /// </summary>
    /// <param name="gigajoules">Energy in gigajoules</param>
    /// <returns>Energy in joules</returns>
    public static double GigajoulesToJoules(double gigajoules)
        => gigajoules * JOULES_PER_GIGAJOULE;

    /// <summary>
    /// Convert joules to gigajoules
    /// </summary>
    /// <param name="joules">Energy in joules</param>
    /// <returns>Energy in gigajoules</returns>
    public static double JoulesToGigajoules(double joules)
        => joules / JOULES_PER_GIGAJOULE;

    #endregion

    #region Helper Methods

    /// <summary>
    /// Format energy value with appropriate unit (J, MJ, GJ) for readability
    /// </summary>
    /// <param name="joules">Energy in joules</param>
    /// <returns>Formatted string (e.g., "34.2 MJ", "1.5 GJ")</returns>
    public static string FormatEnergy(double joules)
    {
        if (Math.Abs(joules) >= JOULES_PER_GIGAJOULE)
            return $"{joules / JOULES_PER_GIGAJOULE:N2} GJ";
        else if (Math.Abs(joules) >= JOULES_PER_MEGAJOULE)
            return $"{joules / JOULES_PER_MEGAJOULE:N2} MJ";
        else
            return $"{joules:N0} J";
    }

    #endregion
}
