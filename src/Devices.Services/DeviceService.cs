using System.Text.Json;
using Devices.API;
using Devices.Entities.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Devices.Services;

public class DeviceService : IDeviceService
{
    private readonly DeviceDbContext _context;

    public DeviceService(DeviceDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<AllDevicesDto>> GetAllDevices(CancellationToken token)
    {
        try
        {
            var devices = await _context.Devices.ToListAsync(token);
            return devices.Select(MapDeviceToDto).ToList();
        }
        catch (Exception ex)
        {
            throw new ApplicationException($"Error getting all devices", ex);
        }
    }

    public async Task<DeviceByIdDto?> GetDeviceById(int id, CancellationToken token)
    {
        try
        {
            var device = await _context.Devices
                .Include(x => x.DeviceType)
                .Include(x => x.DeviceEmployees)
                .ThenInclude(x => x.Employee)
                .ThenInclude(x => x.Person)
                .FirstOrDefaultAsync(x => x.Id == id, token);
            if (device == null)
                return null;
        
            var currentEmployee = device.DeviceEmployees.FirstOrDefault();
            
            var deviceDto = new DeviceByIdDto
            {
                Name = device.Name,
                IsEnabled = device.IsEnabled,
                AdditionalProperties = JsonSerializer.Deserialize<object>(device.AdditionalProperties) ?? new {},
                Type = device.DeviceType.Name
            };
            return deviceDto;
        }
        catch (Exception ex)
        {
            throw new ApplicationException($"Error getting device by id", ex);
        }        
    }

    public async Task<bool> CreateDevice(CreateDeviceDto deviceDto, CancellationToken token)
    {
        if (deviceDto.Name == null) throw new ArgumentException("Device name cannot be null");
        var deviceType = await _context.DeviceTypes.FirstOrDefaultAsync(x => x.Id == deviceDto.TypeId, token);
        if (deviceType == null) 
            throw new KeyNotFoundException($"Device type with id {deviceDto.TypeId} not found");
        
        try
        {
            var device = new Device
            {
                Name = deviceDto.Name,
                IsEnabled = deviceDto.IsEnabled,
                AdditionalProperties = JsonSerializer.Serialize(deviceDto.AdditionalProperties),
                DeviceType = deviceType
            };
            await _context.Devices.AddAsync(device, token);
            await _context.SaveChangesAsync(token);
            return true;
        }
        catch (Exception ex)
        {
            throw new ApplicationException($"Error creating device", ex);
        }
    }

    public async Task<bool> UpdateDevice(int id, CreateDeviceDto deviceDto, CancellationToken token)
    {
        if (deviceDto.Name == null) throw new ArgumentException("Device name cannot be null");

        try
        {
            var device = await _context.Devices
                .Include(x => x.DeviceType)
                .Include(x => x.DeviceEmployees)
                .ThenInclude(x => x.Employee)
                .ThenInclude(x => x.Person)
                .FirstOrDefaultAsync(x => x.Id == id, token);
            if (device == null)
                throw new KeyNotFoundException($"Device with id {id} not found");
            var deviceType = await _context.DeviceTypes.FirstOrDefaultAsync(x => x.Id == deviceDto.TypeId, token);
            if (deviceType == null) 
                throw new KeyNotFoundException($"Device type with id {deviceDto.TypeId} not found");
            
            var newDevice = new Device
            {
                Name = deviceDto.Name,
                IsEnabled = deviceDto.IsEnabled,
                AdditionalProperties = JsonSerializer.Serialize(deviceDto.AdditionalProperties),
                DeviceType = deviceType,
            };
            device.Name = newDevice.Name;
            device.DeviceType = newDevice.DeviceType;
            device.IsEnabled = newDevice.IsEnabled;
            device.AdditionalProperties = newDevice.AdditionalProperties;
            await _context.SaveChangesAsync(token);
            return true;
        }
        catch (Exception ex)
        {
            throw new ApplicationException($"Error updating device", ex);
        }
    }

    public async Task<bool> DeleteDevice(int id, CancellationToken token)
    {
        var device = await _context.Devices.FirstOrDefaultAsync(x => x.Id == id, token);
        if (device == null) throw new KeyNotFoundException($"Device with id {id} not found");
        try
        {
            _context.Devices.Remove(device);
            await _context.SaveChangesAsync(token);
            return true;
        }
        catch (Exception ex)
        {
            throw new ApplicationException($"Error deleting device", ex);
        }
    }

    public async Task<IEnumerable<AllDeviceTypesDto>> GetAllDeviceTypes(CancellationToken token)
    {
        try
        {
            var deviceTypes = await _context.DeviceTypes.ToListAsync(token);
            return deviceTypes.Select(MapDeviceTypeToDto).ToList();

        }
        catch (Exception ex)
        {
            throw new ApplicationException($"Error getting all device types", ex);
        }
    }

    private AllDevicesDto MapDeviceToDto(Device device)
    {
        return new AllDevicesDto
        {
            Id = device.Id,
            Name = device.Name
        };
    }

    private AllDeviceTypesDto MapDeviceTypeToDto(DeviceType deviceType)
    {
        return new AllDeviceTypesDto()
        {
            Id = deviceType.Id,
            Name = deviceType.Name
        };
    }
    
    public async Task<bool> IsDeviceAssignedToUser(int deviceId, int accountId, CancellationToken token)
    {
        var account = await _context.Accounts
            .Include(a => a.Employee)
            .ThenInclude(e => e.DeviceEmployees)
            .FirstOrDefaultAsync(a => a.Id == accountId, token);

        if (account == null || account.Employee == null)
            return false;

        return account.Employee.DeviceEmployees.Any(de => de.DeviceId == deviceId);
    }
}