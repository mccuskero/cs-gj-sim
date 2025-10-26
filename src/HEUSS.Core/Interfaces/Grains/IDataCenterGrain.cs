using HEUSS.Core.Models.Entities;
using Orleans;

namespace HEUSS.Core.Interfaces.Grains;

public interface IDataCenterGrain : IGrainWithGuidKey
{
    Task<DataCenter> GetStateAsync();
    Task SetStateAsync(DataCenter dataCenter);
    Task<double> GetEnergyUseAsync();
    Task TickAsync();
    Task<Guid> GetRegionIdAsync();
    Task SetRegionIdAsync(Guid regionId);
}
