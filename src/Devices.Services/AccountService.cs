using Devices.API;
using Devices.Entities.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using KeyNotFoundException = System.Collections.Generic.KeyNotFoundException;

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
            return accounts.Select(MapAccountToDto).ToList();

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
            var role = await _context.Roles.FirstOrDefaultAsync(r => r.Id == account.RoleId, token);
            var accountDto = new AccountByIdDto
            {
                Username = account.Username,
                RoleName = role.Name
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
        if (string.IsNullOrWhiteSpace(createAccountDto.Username))
            throw new ArgumentException("Username cannot be null or empty");

        if (string.IsNullOrWhiteSpace(createAccountDto.Password))
            throw new ArgumentException("Password cannot be null or empty");
        
        var role = await _context.Roles.FirstOrDefaultAsync(r => r.Id == createAccountDto.RoleId, token);
        if (role == null)
            throw new KeyNotFoundException($"Role with id {createAccountDto.RoleId} not found");
            
        var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Id == createAccountDto.EmployeeId, token);
        if (employee == null)
            throw new KeyNotFoundException($"Employee with id {createAccountDto.EmployeeId} not found");
        try
        {
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

    public async Task<bool> UpdateAccount(int id, UpdateAccountDto updateAccountDto, CancellationToken token)
    {
        if (string.IsNullOrWhiteSpace(updateAccountDto.Username))
            throw new ArgumentException("Username cannot be null or empty");

        if (string.IsNullOrWhiteSpace(updateAccountDto.Password))
            throw new ArgumentException("Password cannot be null or empty");

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

    public async Task<IEnumerable<RoleDto>> GetAllRoles(CancellationToken token)
    {
        try
        {
            var roles = await _context.Roles.ToListAsync(token);
            return roles.Select(MapRoleToDto).ToList();
        }
        catch (Exception ex)
        {
            throw new ApplicationException($"Error getting all roles", ex);
        }
    }
    
    private AllAccountsDto MapAccountToDto(Account account)
    {
        return new AllAccountsDto()
        {
            Id = account.Id,
            Username = account.Username,
        };
    }

    private RoleDto MapRoleToDto(Role role)
    {
        return new RoleDto()
        {
            Id = role.Id,
            Name = role.Name,
        };
    }
}