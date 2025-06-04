using Devices.API;
using Devices.Entities.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Devices.Services;

public class AccountService : IAccountService
{
    private readonly DeviceDbContext _context;
    private readonly PasswordHasher<Account> _passwordHasher = new();

    public AccountService(DeviceDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<AllAccountsDto>> GetAllAccounts(CancellationToken token)
    {
        try
        {
            var accounts = await _context.Accounts.ToListAsync(token);
            return accounts.Select(MapToDto).ToList();

        }
        catch (Exception ex)
        {
            throw new ApplicationException($"Error getting all accounts", ex);
        }
    }

    public async Task<AccountByIdDto?> GetAccountById(int id, CancellationToken token)
    {
        try
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == id, token);
            if (account == null)
                return null;
            var accountDto = new AccountByIdDto
            {
                Username = account.Username,
                Password = account.Password
            };
            return accountDto;
        }
        catch (Exception ex)
        {
            throw new ApplicationException($"Error getting account", ex);
        }
    }

    public async Task<bool> CreateAccount(CreateAccountDto createAccountDto, CancellationToken token)
    {
        try
        {
            var role = await _context.Roles.Where(role => role.Name == createAccountDto.Role)
                .FirstOrDefaultAsync(token);
            if (role == null)
                throw new KeyNotFoundException($"Role {createAccountDto.Role} not found");

            var employee = await _context.Employees.Where(e => e.Person.Email == createAccountDto.Email)
                .FirstOrDefaultAsync(token);
            if (employee == null)
                throw new KeyNotFoundException($"Employee with {createAccountDto.Email} not found");

            var account = new Account
            {
                Username = createAccountDto.Username,
                Password = createAccountDto.Password,
                Employee = employee,
                Role = role
            };

            account.Password = _passwordHasher.HashPassword(account, account.Password);
            await _context.Accounts.AddAsync(account, token);
            await _context.SaveChangesAsync(token);
            return true;
        }
        catch (Exception ex)
        {
            throw new ApplicationException($"Error creating account", ex);
        }
    }
    
    private AllAccountsDto MapToDto(Account account)
    {
        return new AllAccountsDto()
        {
            Id = account.Id,
            Username = account.Username,
            Password = account.Password
        };
    }
}