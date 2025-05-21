using Devices.Entities.DTOs;

namespace Devices.Services;

public interface IEmployeeService
{
    public Task<IEnumerable<AllEmployeesDto>> GetAllEmployees(CancellationToken token);
    public Task<EmployeeByIdDto?> GetEmployeeById(int id, CancellationToken token);
}