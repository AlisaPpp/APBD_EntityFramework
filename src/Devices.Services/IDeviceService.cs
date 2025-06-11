using Devices.Entities.DTOs;

namespace Devices.Services;

public interface IDeviceService
{
    public Task<IEnumerable<AllDevicesDto>> GetAllDevices(CancellationToken token);
    public Task<DeviceByIdDto?> GetDeviceById(int id, CancellationToken token);
    public Task<bool> CreateDevice(CreateDeviceDto deviceDto, CancellationToken token);
    public Task<bool> UpdateDevice(int id, CreateDeviceDto deviceDto, CancellationToken token);
    public Task<bool> DeleteDevice(int id, CancellationToken token);
    public Task<bool> IsDeviceAssignedToUser(int deviceId, int accountId, CancellationToken token);
    public Task<IEnumerable<AllDeviceTypesDto>> GetAllDeviceTypes(CancellationToken token);
    public Task<string?> GetDeviceTypeNameById(int id, CancellationToken token);
}