using Devices.API;
using Devices.Entities.DTOs;

namespace Devices.Services;

public interface IAccountService
{
    public Task<IEnumerable<AllAccountsDto>> GetAllAccounts(CancellationToken token);
    public Task<AccountByIdDto?> GetAccountById(int id, CancellationToken token);
    public Task<bool> CreateAccount(CreateAccountDto createAccountDto, CancellationToken token);
}