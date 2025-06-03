using Devices.API;
using Devices.Entities.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Devices.Services;

public class EmployeeService : IEmployeeService
{
    private readonly DeviceDbContext _context;

    public EmployeeService(DeviceDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<AllEmployeesDto>> GetAllEmployees(CancellationToken token)
    {
        try
        {
            var employees = await _context.Employees
                .Include(p => p.Person)
                .ToListAsync(token);
            return employees.Select(MapToDto).ToList();

        }
        catch (Exception ex)
        {
            throw new ApplicationException($"Failed to get all employees", ex);
        }
    }

    public async Task<EmployeeByIdDto?> GetEmployeeById(int id, CancellationToken token)
    {
        try
        {
            var employee = await _context.Employees
                .Include(p => p.Person)
                .Include(p => p.Position)
                .FirstOrDefaultAsync(p => p.Id == id, token);
            if (employee == null) return null;
            return new EmployeeByIdDto
            {
                PersonInfo = new PersonInfoDto
                {
                    Id = employee.Id,
                    FirstName = employee.Person.FirstName,
                    LastName = employee.Person.LastName,
                    MiddleName = employee.Person.MiddleName,
                    Email = employee.Person.Email,
                    PhoneNumber = employee.Person.PhoneNumber,
                    PassportNumber = employee.Person.PassportNumber
                },
                Position = new PositionDto
                {
                    Id = employee.Position.Id,
                    Name = employee.Position.Name,
                },
                HireDate = employee.HireDate,
                Salary = employee.Salary,
            };

        }
        catch (Exception ex)
        {
            throw new ApplicationException($"Failed to get employee by id", ex);
        }
    }
    
    private AllEmployeesDto MapToDto(Employee employee)
    {
        return new AllEmployeesDto
        {
            Id = employee.Id,
            FullName = $"{employee.Person.FirstName} {employee.Person.MiddleName} {employee.Person.LastName}"
        };
    }

}