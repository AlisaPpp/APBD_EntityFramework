using Devices.API;
using Microsoft.EntityFrameworkCore;

namespace Devices.Repositories;

public class EmployeeRepository : IEmployeeRepository
{
    private DeviceDbContext _context;

    public EmployeeRepository(DeviceDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Employee>> GetAllEmployeesAsync(CancellationToken token)
    {
        try
        {
            return await _context.Employees
                .Include(p => p.Person)
                .ToListAsync(token);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to get all employees", ex);
        }
    }

    public async Task<Employee?> GetEmployeeByIdAsync(int id, CancellationToken token)
    {
        try
        {
            return await _context.Employees
                .Include(p => p.Person)
                .Include(p => p.Position)
                .FirstOrDefaultAsync(p => p.Id == id, token);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to get employee by id", ex);
        }
    }
}