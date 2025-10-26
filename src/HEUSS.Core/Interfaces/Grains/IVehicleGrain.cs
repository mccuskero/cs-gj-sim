using HEUSS.Core.Models.Entities;

namespace HEUSS.Core.Interfaces.Grains;

/// <summary>
/// Orleans grain interface for Vehicle entity
/// </summary>
public interface IVehicleGrain : Orleans.IGrainWithGuidKey
{
    Task<Vehicle> GetStateAsync();
    Task SetStateAsync(Vehicle vehicle);
    Task<double> GetEnergyUseAsync();
    Task TickAsync();
}
