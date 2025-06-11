using Devices.Entities.DTOs;
using Microsoft.AspNetCore.Mvc;
using Devices.Services;
using Devices.Services.TokenService;

namespace Devices.API;

[ApiController]
[Route("api/employees/[controller]")]
public class EmployeeController : ControllerBase
{
    private readonly IEmployeeService _employeeService;
    private readonly ILogger<EmployeeController> _logger;

    public EmployeeController(IEmployeeService employeeService, ILogger<EmployeeController> logger)
    {
        _employeeService = employeeService;
        _logger = logger;
    }

    [HttpGet]
    [Route("/api/employees")]
    public async Task<IResult> GetAllEmployees(CancellationToken token)
    {
        try
        {
            var employees = await _employeeService.GetAllEmployees(token);
            return Results.Ok(employees);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while getting all employees");
            return Results.Problem(ex.Message);
        }
    }

    [HttpGet]
    [Route("/api/employees/{id}")]
    public async Task<IResult> GetEmployeeById(int id, CancellationToken token)
    {
        try
        {
            var employee = await _employeeService.GetEmployeeById(id, token);
            if (employee == null)
            {
                _logger.LogError($"Employee with id {id} not found");
                return Results.NotFound($"Employee with Id {id} not found.");
            }
            return Results.Ok(employee);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while getting employee with id {id}");
            return Results.Problem(ex.Message);
        }
    }

    [HttpPost]
    [Route("/api/employees")]
    public async Task<IResult> CreateEmployee(CreateEmployeeDto createEmployeeDto, CancellationToken token)
    {
        try
        {
            await _employeeService.CreateEmployee(createEmployeeDto, token);
            return Results.Created();
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, $"When creating employee, position id {createEmployeeDto.PositionId} was not found");
            return Results.NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while creating employee");
            return Results.Problem(ex.Message);
        }
    }

    [HttpGet]
    [Route("/api/positions")]
    public async Task<IResult> GetAllPositions(CancellationToken token)
    {
        try
        {
            var positions = await _employeeService.GetAllPositions(token);
            return Results.Ok(positions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while getting all positions");
            return Results.Problem(ex.Message);
        }
    }
}