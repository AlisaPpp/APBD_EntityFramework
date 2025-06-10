using System.Security.Claims;
using Devices.Entities.DTOs;
using Microsoft.AspNetCore.Mvc;
using Devices.Services;
using Microsoft.AspNetCore.Authorization;

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
    [Authorize(Roles = "Admin")]
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
    [Authorize(Roles = "Admin,User")]
    public async Task<IResult> GetDeviceById(int id, CancellationToken token)
    {
        try
        {
            var currentUser = User.FindFirst(ClaimTypes.Actor)?.Value;
            var currentRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (currentRole == "Admin")
            {
                var device = await _deviceService.GetDeviceById(id, token);
                if (device == null)
                    return Results.NotFound($"Device with Id {id} not found.");
                return Results.Ok(device);
            }
            
            var hasAccess = await _deviceService.IsDeviceAssignedToUser(id, int.Parse(currentUser), token);
            if (!hasAccess)
                return Results.Unauthorized();
            var userDevice = await _deviceService.GetDeviceById(id, token);
            return Results.Ok(userDevice);
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message);
        }
    }

    [HttpPost]
    [Route("/api/devices")]
    [Authorize(Roles = "Admin")]
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
    [Authorize(Roles = "Admin, User")]
    public async Task<IResult> UpdateDevice(int id, CreateDeviceDto deviceDto, CancellationToken token)
    {
        try
        {
            var currentUser = User.FindFirst(ClaimTypes.Actor)?.Value;
            var currentRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (currentRole != "Admin")
            {
                var hasAccess = await _deviceService.IsDeviceAssignedToUser(id, int.Parse(currentUser), token);
                if (!hasAccess)
                    return Results.Unauthorized();
            }
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
    [Authorize(Roles = "Admin")]
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

    [HttpGet]
    [Route("/api/devices/types")]
    [Authorize(Roles = "Admin")]
    public async Task<IResult> GetAllDeviceTypes(CancellationToken token)
    {
        try
        {
            var deviceTypes = await _deviceService.GetAllDeviceTypes(token);
            return Results.Ok(deviceTypes);
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message);
        }
    }
}