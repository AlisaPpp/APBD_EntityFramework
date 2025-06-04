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
    [Authorize(Roles = "Admin")]
    [Route("/api/accounts/{id}")]
    public async Task<IResult> GetAccountById(int id, CancellationToken token)
    {
        try
        {
            var account = await _accountService.GetAccountById(id, token);
            if (account == null)
                return Results.NotFound($"Account with Id {id} not found.");
            return Results.Ok(account);
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
        catch (Exception ex)
        {
            return Results.Problem(ex.Message);
        }
    }
}