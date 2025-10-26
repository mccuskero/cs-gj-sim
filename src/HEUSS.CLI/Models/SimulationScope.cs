namespace HEUSS.CLI.Models;

/// <summary>
/// Defines the scope of the simulation run
/// </summary>
public enum SimulationScope
{
    /// <summary>
    /// Run simulation for all nations globally
    /// </summary>
    Global,

    /// <summary>
    /// Run simulation for a specific nation
    /// </summary>
    Nation,

    /// <summary>
    /// Run simulation for a specific region within a nation
    /// </summary>
    Region
}
