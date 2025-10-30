namespace HEUSS.Core.Enums;

/// <summary>
/// Defines the socioeconomic level of a person, which determines their energy consumption patterns.
/// Energy consumption varies by ~100x across archetypes.
/// </summary>
public enum SocioeconomicArchetype
{
    /// <summary>
    /// Extreme poverty: 6-12 MJ/day (100% biological energy)
    /// No owned house, no vehicle, no electricity access
    /// </summary>
    Subsistence = 1,

    /// <summary>
    /// Low income: 12-30 MJ/day (67% biological, 33% technological)
    /// Basic shelter, no vehicle, limited electricity (2-4 hours/day)
    /// </summary>
    LowIncome = 2,

    /// <summary>
    /// Lower-middle income: 30-80 MJ/day (25% biological, 75% technological)
    /// Small apartment/house, may own motorcycle, regular electricity
    /// </summary>
    LowerMiddle = 3,

    /// <summary>
    /// Middle income: 80-200 MJ/day (8% biological, 92% technological)
    /// Owned/rented house, 1 car, 24-hour electricity, partial climate control
    /// </summary>
    Middle = 4,

    /// <summary>
    /// Upper-middle income: 200-400 MJ/day (5% biological, 95% technological)
    /// Owned house, 1-2 cars, full climate control, multiple appliances
    /// </summary>
    UpperMiddle = 5,

    /// <summary>
    /// Affluent: 400-1000 MJ/day (2% biological, 98% technological)
    /// Large house(s), 2-3+ vehicles, extensive energy consumption
    /// </summary>
    Affluent = 6
}
