using Devices.API;
using Devices.Entities.DTOs;

namespace Devices.Services;

public interface IDeviceService
{
    public Task<List<AllDevicesDto>> GetAllDevices(CancellationToken token);
    public Task<DeviceByIdDto?> GetDeviceById(int id, CancellationToken token);
    public Task<bool> CreateDevice(CreateDeviceDto deviceDto, CancellationToken token);
    public Task<bool> UpdateDevice(int id, CreateDeviceDto deviceDto, CancellationToken token);
    public Task<bool> DeleteDevice(int id, CancellationToken token);
}