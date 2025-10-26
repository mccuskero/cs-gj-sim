using HEUSS.Core.Models.Entities;

namespace HEUSS.Core.Interfaces.Grains;

/// <summary>
/// Orleans grain interface for House entity
/// </summary>
public interface IHouseGrain : Orleans.IGrainWithGuidKey
{
    Task<House> GetStateAsync();
    Task SetStateAsync(House house);
    Task<double> GetEnergyUseAsync();
    Task TickAsync();
}
