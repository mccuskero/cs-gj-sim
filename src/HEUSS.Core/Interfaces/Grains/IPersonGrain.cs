using HEUSS.Core.Models.Entities;

namespace HEUSS.Core.Interfaces.Grains;

/// <summary>
/// Orleans grain interface for Person entity
/// </summary>
public interface IPersonGrain : Orleans.IGrainWithGuidKey
{
    /// <summary>
    /// Get the current state of the person
    /// </summary>
    Task<Person> GetStateAsync();

    /// <summary>
    /// Update person state
    /// </summary>
    Task SetStateAsync(Person person);

    /// <summary>
    /// Get current daily energy consumption
    /// </summary>
    Task<double> GetEnergyUseAsync();

    /// <summary>
    /// Process a simulation tick (update state based on time progression)
    /// </summary>
    Task TickAsync();
}
