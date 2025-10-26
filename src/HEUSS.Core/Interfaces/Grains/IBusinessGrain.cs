using HEUSS.Core.Models.Entities;

namespace HEUSS.Core.Interfaces.Grains;

/// <summary>
/// Orleans grain interface for Business entity
/// </summary>
public interface IBusinessGrain : Orleans.IGrainWithGuidKey
{
    Task<Business> GetStateAsync();
    Task SetStateAsync(Business business);
    Task<double> GetEnergyUseAsync();
    Task TickAsync();
}
