namespace HEUSS.Core.Enums;

/// <summary>
/// Climate classification affecting heating and cooling energy requirements
/// </summary>
public enum ClimateType
{
    /// <summary>
    /// Tropical climate: minimal heating, high cooling needs
    /// </summary>
    Tropical,

    /// <summary>
    /// Arid/Desert: extreme temperatures, high cooling needs
    /// </summary>
    Arid,

    /// <summary>
    /// Temperate: moderate heating and cooling needs
    /// </summary>
    Temperate,

    /// <summary>
    /// Continental: cold winters, warm summers, high heating needs
    /// </summary>
    Continental,

    /// <summary>
    /// Polar: extreme cold, very high heating needs
    /// </summary>
    Polar,

    /// <summary>
    /// Mediterranean: mild winters, hot summers, moderate energy needs
    /// </summary>
    Mediterranean
}
