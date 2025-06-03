using Devices.API;
using Devices.Services;
using Devices.Entities.DTOs;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("PersonalDatabase") ?? 
                       throw new InvalidOperationException("Personal connection string not found");
builder.Services.AddDbContext<DeviceDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddTransient<IDeviceService, DeviceService>();
builder.Services.AddTransient<IEmployeeService, EmployeeService>();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/api/devices", async (IDeviceService deviceService, CancellationToken token) =>
{
    try
    {
        var devices = await deviceService.GetAllDevices(token);
        return Results.Ok(devices);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

app.MapGet("/api/devices/{id}", async (IDeviceService deviceService, int id, CancellationToken token) =>
{
    try
    {
        var device = await deviceService.GetDeviceById(id, token);
        return Results.Ok(device);
    }
    catch (KeyNotFoundException ex)
    {
        return Results.NotFound(ex.Message);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

app.MapPost("/api/devices", async (IDeviceService deviceService, CancellationToken token, CreateDeviceDto deviceDto) =>
{
    try
    {
        await deviceService.CreateDevice(deviceDto, token);
        return Results.Created();
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(ex.Message);
    }
});

app.MapPut("/api/devices/{id}", async (IDeviceService deviceService, CancellationToken token, int id, CreateDeviceDto deviceDto) =>
{
    try
    {
        await deviceService.UpdateDevice(id, deviceDto, token);
        return Results.Ok();
    }
    catch (KeyNotFoundException ex)
    {
        return Results.NotFound(ex.Message);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(ex.Message);
    }
});

app.MapDelete("/api/devices/{id}", async (IDeviceService deviceService, int id, CancellationToken token) =>
{
    try
    {
        await deviceService.DeleteDevice(id, token);
        return Results.Ok();
    }
    catch (KeyNotFoundException ex)
    {
        return Results.NotFound(ex.Message);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

app.MapGet("/api/employees", async (IEmployeeService employeeService, CancellationToken token) =>
{
    try
    {
        var employees = await employeeService.GetAllEmployees(token);
        return Results.Ok(employees);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

app.MapGet("/api/employees/{id}", async (IEmployeeService employeeService, int id, CancellationToken token) =>
{
    try
    {
        var employee = await employeeService.GetEmployeeById(id, token);
        if (employee == null)
            return Results.NotFound($"Employee with Id {id} not found.");
        return Results.Ok(employee);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

app.Run();

