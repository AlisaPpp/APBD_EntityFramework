using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Devices.Services;
using Devices.Entities;
using Devices.Entities.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace Devices.API;

[ApiController]
[Route("/api/accounts/[controller]")]
public class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;
    private readonly ILogger<AccountController> _logger;

    public AccountController(IAccountService accountService, ILogger<AccountController> logger)
    {
        _accountService = accountService;
        _logger = logger;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    [Route("/api/accounts")]
    public async Task<IResult> GetAllAccounts(CancellationToken token)
    {
        try
        {
            var accounts = await _accountService.GetAllAccounts(token);
            return Results.Ok(accounts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting all accounts.");
            return Results.Problem(ex.Message);
        }
    }
    
    [HttpGet]
    [Authorize(Roles = "Admin, User")]
    [Route("/api/accounts/{id}")]
    public async Task<IResult> GetAccountById(int id, CancellationToken token)
    {
        try
        {
            var currentUser = User.FindFirst(ClaimTypes.Actor)?.Value;
            var currentRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (currentRole == "Admin" || currentUser.Equals(id.ToString()))
            {
                var account = await _accountService.GetAccountById(id, token);
                if (account == null)
                {
                    _logger.LogWarning($"Account with id {id} not found");
                    return Results.NotFound($"Account with Id {id} not found.");
                }
                return Results.Ok(account);
            }
            return Results.Unauthorized();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while getting account by id {id}");
            return Results.Problem(ex.Message);
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [Route("/api/accounts")]
    public async Task<IResult> AddAccount(CreateAccountDto accountDto, CancellationToken token)
    {
        try
        {
            await _accountService.CreateAccount(accountDto, token);
            return Results.Created();
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex,
                $"Employee with id {accountDto.EmployeeId}/ Role with id {accountDto.RoleId} not found when adding new account");
            return Results.NotFound(ex.Message);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, $"Username or password cannot be null or empty");
            return Results.BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while adding new account");
            return Results.Problem(ex.Message);
        }
    }

    [HttpPut]
    [Authorize(Roles = "Admin,User")]
    [Route("/api/accounts/{id}")]
    public async Task<IResult> UpdateAccount(int id, UpdateAccountDto accountDto, CancellationToken token)
    {
        try
        {
            var currentUserId = User.FindFirst(ClaimTypes.Actor)?.Value;
            var currentRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (currentRole == "Admin" || currentUserId.Equals(id.ToString()))
            {
                await _accountService.UpdateAccount(id, accountDto, token);
                return Results.Ok();
            }
            
            _logger.LogWarning($"User with id {int.Parse(currentUserId)} has no authority to update account with id {id}");
            return Results.Unauthorized();
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, $"Account with id {id} not found");
            return Results.NotFound(ex.Message);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, $"Username or password cannot be null or empty");
            return Results.BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while updating account with id {id}");
            return Results.Problem(ex.Message);
        }
    }

    [HttpDelete]
    [Authorize(Roles = "Admin")]
    [Route("/api/accounts/{id}")]
    public async Task<IResult> DeleteAccount(int id, CancellationToken token)
    {
        try
        {
            await _accountService.DeleteAccount(id, token);
            return Results.Ok();
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, $"Account with id {id} not found");
            return Results.NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while deleting account with id {id}");
            return Results.Problem(ex.Message);
        }
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    [Route("/api/roles")]
    public async Task<IResult> GetAllRoles(CancellationToken token)
    {
        try
        {
            var roles = await _accountService.GetAllRoles(token);
            return Results.Ok(roles);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while getting all roles");
            return Results.Problem(ex.Message);
        }
    }
}