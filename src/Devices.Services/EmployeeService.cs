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
            return employees.Select(MapEmployeeToDto).ToList();

        }
        catch (Exception ex)
        {
            throw new ApplicationException($"Error getting all employees", ex);
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
            if (employee == null) 
                return null;
            return new EmployeeByIdDto
            {
                Person = new PersonInfoDto
                {
                    FirstName = employee.Person.FirstName,
                    LastName = employee.Person.LastName,
                    MiddleName = employee.Person.MiddleName,
                    Email = employee.Person.Email,
                    PhoneNumber = employee.Person.PhoneNumber,
                    PassportNumber = employee.Person.PassportNumber
                },
                Salary = employee.Salary,
                Position = employee.Position.Name,
                HireDate = employee.HireDate
            };

        }
        catch (Exception ex)
        {
            throw new ApplicationException($"Error getting employee by id", ex);
        }
    }

    public async Task<bool> CreateEmployee(CreateEmployeeDto createEmployeeDto, CancellationToken token)
    {
        var position = await _context.Positions.FirstOrDefaultAsync(p => p.Id == createEmployeeDto.PositionId, token);
        if (position == null)
            throw new KeyNotFoundException($"Position with id {createEmployeeDto.PositionId} not found");
        try
        {
            var person = new Person()
            {
                PassportNumber = createEmployeeDto.Person.PassportNumber,
                FirstName = createEmployeeDto.Person.FirstName,
                MiddleName = createEmployeeDto.Person.MiddleName,
                LastName = createEmployeeDto.Person.LastName,
                PhoneNumber = createEmployeeDto.Person.PhoneNumber,
                Email = createEmployeeDto.Person.Email
            };
            var employee = new Employee()
            {
                Person = person,
                Salary = createEmployeeDto.Salary,
                PositionId = createEmployeeDto.PositionId,
            };
            
            _context.People.Add(person);
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync(token);
            return true;
        }
        catch (Exception ex)
        {
            throw new ApplicationException($"Error creating employee", ex);
        }
    }

    public async Task<IEnumerable<PositionDto>> GetAllPositions(CancellationToken token)
    {
        try
        {
            var positions = await _context.Positions.ToListAsync(token);
            return positions.Select(MapPositionToDto).ToList();
        }
        catch (Exception ex)
        {
            throw new ApplicationException($"Error getting all positions", ex);
        }
    }
    
    private AllEmployeesDto MapEmployeeToDto(Employee employee)
    {
        return new AllEmployeesDto
        {
            Id = employee.Id,
            FullName = $"{employee.Person.FirstName} {employee.Person.MiddleName} {employee.Person.LastName}"
        };
    }

    private PositionDto MapPositionToDto(Position position)
    {
        return new PositionDto()
        {
            Id = position.Id,
            Name = position.Name,
        };
    }

}