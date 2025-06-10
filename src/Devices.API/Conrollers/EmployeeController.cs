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

    public EmployeeController(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
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
                return Results.NotFound($"Employee with Id {id} not found.");
            return Results.Ok(employee);
        }
        catch (Exception ex)
        {
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
            return Results.NotFound(ex.Message);
        }
        catch (Exception ex)
        {
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
            return Results.Problem(ex.Message);
        }
    }
}