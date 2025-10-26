using HEUSS.Core.Models.Entities;
using Orleans;

namespace HEUSS.Core.Interfaces.Grains;

public interface IFarmGrain : IGrainWithGuidKey
{
    Task<Farm> GetStateAsync();
    Task SetStateAsync(Farm farm);
    Task<double> GetEnergyUseAsync();
    Task<double> GetFoodOutputEnergyAsync();
    Task<double> GetEnergyReturnOnInvestmentAsync();
    Task TickAsync();
    Task<Guid> GetRegionIdAsync();
    Task SetRegionIdAsync(Guid regionId);
}
