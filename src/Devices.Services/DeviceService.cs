using Devices.API;
using Devices.Repositories;
using Devices.Entities;
using Devices.Entities.DTOs;

namespace Devices.Services;

public class DeviceService : IDeviceService
{
    private readonly IDeviceRepository _deviceRepository;

    public DeviceService(IDeviceRepository deviceRepository)
    {
        _deviceRepository = deviceRepository;
    }

    public async Task<List<AllDevicesDto>> GetAllDevices(CancellationToken token)
    {
        var devices = await _deviceRepository.GetAllDevicesAsync(token);
        return devices.Select(MapToDto).ToList();
    }

    public async Task<DeviceByIdDto?> GetDeviceById(int id, CancellationToken token)
    {
        var device = await _deviceRepository.GetDeviceByIdAsync(id, token);
        if (device == null) return null;
        
        var currentEmployee = device.DeviceEmployees.FirstOrDefault(e => e.EmployeeId == id);

        var deviceDto = new DeviceByIdDto
        {
            Name = device.Name,
            DeviceType = device.DeviceType.Name,
            IsEnabled = device.IsEnabled,
            AdditionalProperties = device.AdditionalProperties.ToString() ?? "{}",
            CurrentEmployee = currentEmployee != null
                ? new AllEmployeesDto
                {
                    Id = currentEmployee.Id,
                    FullName =
                        $"{currentEmployee.Employee.Person.FirstName} {currentEmployee.Employee.Person.MiddleName} {currentEmployee.Employee.Person.LastName}"
                }
                : null
        };
        return deviceDto;
    }

    public async Task<bool> CreateDevice(CreateDeviceDto deviceDto, CancellationToken token)
    {
        if (deviceDto.Name == null) throw new ArgumentException("Device name cannot be null");
        if (deviceDto.DeviceType == null) throw new ArgumentException("Device type cannot be null");
        
        var deviceType = await _deviceRepository.GetDeviceTypeByNameAsync(deviceDto.DeviceType, token);
        if (deviceType == null) throw new ArgumentException("Device type cannot be null");
        var device = new Device
        {
            Name = deviceDto.Name,
            DeviceType = deviceType,
            IsEnabled = deviceDto.IsEnabled,
            AdditionalProperties = deviceDto.AdditionalProperties.ToString() ?? "{}"
        };
        
        return await _deviceRepository.CreateDeviceAsync(device, token);
    }

    public async Task<bool> UpdateDevice(int id, CreateDeviceDto deviceDto, CancellationToken token)
    {
        if (await _deviceRepository.GetDeviceByIdAsync(id, token) == null) 
            throw new KeyNotFoundException("Device not found");
        
        if (deviceDto.Name == null) throw new ArgumentException("Device name cannot be null");
        if (deviceDto.DeviceType == null) throw new ArgumentException("Device type cannot be null");
        var deviceType = await _deviceRepository.GetDeviceTypeByNameAsync(deviceDto.DeviceType, token);
        if (deviceType == null) throw new ArgumentException("Device type cannot be null");
        var device = new Device
        {
            Name = deviceDto.Name,
            DeviceType = deviceType,
            IsEnabled = deviceDto.IsEnabled,
            AdditionalProperties = deviceDto.AdditionalProperties.ToString() ?? "{}"
        };
        return await _deviceRepository.UpdateDeviceAsync(id, device, token);
    }

    public async Task<bool> DeleteDevice(int id, CancellationToken token)
    {
        if (await _deviceRepository.GetDeviceByIdAsync(id, token) == null) 
            throw new KeyNotFoundException("Device not found");
        return await _deviceRepository.DeleteDeviceAsync(id, token);
    }


    private AllDevicesDto MapToDto(Device device)
    {
        return new AllDevicesDto
        {
            Id = device.Id,
            Name = device.Name
        };
    }
}