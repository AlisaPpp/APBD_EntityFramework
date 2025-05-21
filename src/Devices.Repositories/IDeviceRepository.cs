using Devices.API;

namespace Devices.Repositories;

public interface IDeviceRepository
{
    public Task<IEnumerable<Device>> GetAllDevicesAsync(CancellationToken token);
    public Task<Device?> GetDeviceByIdAsync(int id, CancellationToken token);
    public Task<bool> CreateDeviceAsync(Device device, CancellationToken token);
    public Task<bool> UpdateDeviceAsync(int id, Device device, CancellationToken token);
    public Task<bool> DeleteDeviceAsync(int id, CancellationToken token);
    public Task<DeviceType> GetDeviceTypeByNameAsync(string name, CancellationToken token);
}