using Devices.API;
using Devices.Entities.DTOs;
using Devices.Repositories;

namespace Devices.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _employeeRepository;

    public EmployeeService(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }

    public async Task<IEnumerable<AllEmployeesDto>> GetAllEmployees(CancellationToken token)
    {
        var employees = await _employeeRepository.GetAllEmployeesAsync(token);
        return employees.Select(MapToDto).ToList();
    }

    public async Task<EmployeeByIdDto?> GetEmployeeById(int id, CancellationToken token)
    {
        var employee = await _employeeRepository.GetEmployeeByIdAsync(id, token);
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
    
    private AllEmployeesDto MapToDto(Employee employee)
    {
        return new AllEmployeesDto
        {
            Id = employee.Id,
            FullName = $"{employee.Person.FirstName} {employee.Person.MiddleName} {employee.Person.LastName}"
        };
    }

}