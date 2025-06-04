using Devices.Entities.DTOs;
using Microsoft.AspNetCore.Mvc;
using Devices.Services;
namespace Devices.API;

[ApiController]
[Route("api/devices/[controller]")]
public class DeviceController : ControllerBase
{
    private readonly IDeviceService _deviceService;

    public DeviceController(IDeviceService deviceService)
    {
        _deviceService = deviceService;
    }

    [HttpGet]
    [Route("/api/devices")]
    public async Task<IResult> GetAllDevices(CancellationToken token)
    {
        try
        {
            var devices = await _deviceService.GetAllDevices(token);
            return Results.Ok(devices);
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message);
        }
    }

    [HttpGet]
    [Route("/api/devices/{id}")]
    public async Task<IResult> GetDeviceById(int id, CancellationToken token)
    {
        try
        {
            var device = await _deviceService.GetDeviceById(id, token);
            if (device == null)
                return Results.NotFound($"Device with Id {id} not found.");
            return Results.Ok(device);
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message);
        }
    }

    [HttpPost]
    [Route("/api/devices")]
    public async Task<IResult> AddDevice(CreateDeviceDto deviceDto, CancellationToken token)
    {
        try
        {
            await _deviceService.CreateDevice(deviceDto, token);
            return Results.Created();
        }
        catch (ArgumentException ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }

    [HttpPut]
    [Route("/api/devices/{id}")]
    public async Task<IResult> UpdateDevice(int id, CreateDeviceDto deviceDto, CancellationToken token)
    {
        try
        {
            await _deviceService.UpdateDevice(id, deviceDto, token);
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
    }

    [HttpDelete]
    [Route("/api/devices/{id}")]
    public async Task<IResult> DeleteDevice(int id, CancellationToken token)
    {
        try
        {
            await _deviceService.DeleteDevice(id, token);
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
    }
}