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
                throw new ArgumentException($"Role {createAccountDto.Role} is not valid");

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

    public async Task<bool> UpdateAccount(int id, bool isAdmin, CreateAccountDto updateAccountDto, CancellationToken token)
    {
        if (updateAccountDto.Username == null)
            throw new ArgumentException("Username cannot be null");
        if (updateAccountDto.Password == null)
            throw new ArgumentException("Password cannot be null");

        try
        {
            var account = await _context.Accounts
                .Include(a => a.Role)
                .Include(a => a.Employee)
                .FirstOrDefaultAsync(a => a.Id == id, token);
            if (account == null)
                throw new KeyNotFoundException($"Account with id {id} not found");

            account.Username = updateAccountDto.Username;
            account.Password = _passwordHasher.HashPassword(account, updateAccountDto.Password);

            if (isAdmin)
            {
                var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == updateAccountDto.Role, token);
                if (role == null)
                    throw new ArgumentException($"Role {updateAccountDto.Role} is not valid");
                account.Role = role;
            }

            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Person.Email == updateAccountDto.Email, token);
            if (employee == null)
                throw new KeyNotFoundException($"Employee with {updateAccountDto.Email} not found");
            account.Employee = employee;

            await _context.SaveChangesAsync(token);
            return true;
        }
        catch (Exception ex)
        {
            throw new ApplicationException($"Error updating account", ex);
        }
    }


    public async Task<bool> DeleteAccount(int id, CancellationToken token)
    {
        var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == id, token);
        if (account == null)
            throw new KeyNotFoundException($"Account with id {id} not found");
        try
        {
            _context.Accounts.Remove(account);
            await _context.SaveChangesAsync(token);
            return true;
        }
        catch (Exception ex)
        {
            throw new ApplicationException($"Error deleting account", ex);
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