using Devices.API;
using Microsoft.EntityFrameworkCore;

namespace Devices.Repositories;

public class DeviceRepository : IDeviceRepository
{
    private DeviceDbContext _context;

    public DeviceRepository(DeviceDbContext context)
    {
        _context = context;
    }

    public async Task<List<Device>> GetAllDevicesAsync(CancellationToken token)
    {
        try
        {
            return await _context.Devices.ToListAsync(token);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error getting all devices", ex);
        }
    }

    public async Task<Device?> GetDeviceByIdAsync(int id, CancellationToken token)
    {
        try
        {
            return await _context.Devices
                .Include(x => x.DeviceType)
                .Include(x => x.DeviceEmployees)
                .ThenInclude(x => x.Employee)
                .ThenInclude(x => x.Person)
                .FirstOrDefaultAsync(x => x.Id == id, token);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error getting device by id", ex);
        }
    }

    public async Task<bool> CreateDeviceAsync(Device device, CancellationToken token)
    {
        try
        {
            await _context.Devices.AddAsync(device, token);
            await _context.SaveChangesAsync(token);
            return true;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error creating device", ex);
        }
    }

    public async Task<bool> UpdateDeviceAsync(int id, Device newDevice, CancellationToken token)
    {
        try
        {
            var device = await GetDeviceByIdAsync(id, token);
            device.Name = newDevice.Name;
            device.DeviceType = newDevice.DeviceType;
            device.IsEnabled = newDevice.IsEnabled;
            device.AdditionalProperties = newDevice.AdditionalProperties;
            await _context.SaveChangesAsync(token);
            return true;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error updating device", ex);
        }
    }

    public async Task<bool> DeleteDeviceAsync(int id, CancellationToken token)
    {
        try
        {
            var device = await _context.Devices.FirstOrDefaultAsync(x => x.Id == id, token);
            _context.Devices.Remove(device);
            await _context.SaveChangesAsync(token);
            return true;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error deleting device", ex);
        }
    }

    public async Task<DeviceType> GetDeviceTypeByNameAsync(string name, CancellationToken token)
    {
        try
        {
            return await _context.DeviceTypes.FirstOrDefaultAsync(x => x.Name == name, token);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error getting device type", ex);
        }
    }
}