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

    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
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
                    return Results.NotFound($"Account with Id {id} not found.");
                return Results.Ok(account);
            }
            return Results.Unauthorized();
        }
        catch (Exception ex)
        {
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
            return Results.NotFound(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return Results.BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message);
        }
    }

    [HttpPut]
    [Authorize(Roles = "Admin,User")]
    [Route("/api/accounts/{id}")]
    public async Task<IResult> UpdateAccount(int id, AccountByIdDto accountDto, CancellationToken token)
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
            return Results.Unauthorized();
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
            return Results.NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message);
        }
    }
}