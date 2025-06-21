using Application.DTOs;

namespace Application.Interfaces
{
    public interface IAccountService
    {
        Task<IEnumerable<ChartOfAccountDto>> GetChartOfAccountsAsync();
        Task<AccountDto?> GetAccountByIdAsync(int accountId);
        Task CreateAccountAsync(AccountDto accountDto);
        Task UpdateAccountAsync(AccountDto accountDto);
        Task<bool> CanDeleteAccountAsync(int accountId);
        Task DeleteAccountAsync(int accountId);
    }
} 