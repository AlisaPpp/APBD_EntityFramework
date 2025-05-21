using Devices.API;

namespace Devices.Repositories;

public interface IEmployeeRepository
{
    public Task<List<Employee>> GetAllEmployeesAsync(CancellationToken token);
    public Task<Employee?> GetEmployeeByIdAsync(int id, CancellationToken token);
}