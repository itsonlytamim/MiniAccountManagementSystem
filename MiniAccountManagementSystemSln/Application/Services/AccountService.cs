using Application.DTOs;
using Application.Interfaces;
using Core.Entities;

namespace Application.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;

        public AccountService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task<IEnumerable<ChartOfAccountDto>> GetChartOfAccountsAsync()
        {
            var rootAccounts = await _accountRepository.GetAllHierarchicalAsync();
            return MapToChartOfAccountDto(rootAccounts, 0);
        }

        public async Task<AccountDto?> GetAccountByIdAsync(int accountId)
        {
            var account = await _accountRepository.GetByIdAsync(accountId);
            if (account == null) return null;

            return new AccountDto
            {
                AccountId = account.AccountId,
                AccountCode = account.AccountCode,
                AccountName = account.AccountName,
                ParentAccountId = account.ParentAccountId
            };
        }

        public async Task CreateAccountAsync(AccountDto accountDto)
        {
            var account = new Account
            {
                AccountCode = accountDto.AccountCode,
                AccountName = accountDto.AccountName,
                ParentAccountId = accountDto.ParentAccountId
            };
            await _accountRepository.CreateAsync(account);
        }

        public async Task UpdateAccountAsync(AccountDto accountDto)
        {
            var account = new Account
            {
                AccountId = accountDto.AccountId,
                AccountCode = accountDto.AccountCode,
                AccountName = accountDto.AccountName,
                ParentAccountId = accountDto.ParentAccountId
            };
            await _accountRepository.UpdateAsync(account);
        }

        public async Task<bool> CanDeleteAccountAsync(int accountId)
        {
            return !await _accountRepository.HasChildrenAsync(accountId);
        }

        public async Task DeleteAccountAsync(int accountId)
        {
            await _accountRepository.DeleteAsync(accountId);
        }

        private List<ChartOfAccountDto> MapToChartOfAccountDto(IEnumerable<Account> accounts, int level)
        {
            var dtoList = new List<ChartOfAccountDto>();
            foreach (var account in accounts)
            {
                var dto = new ChartOfAccountDto
                {
                    AccountId = account.AccountId,
                    AccountCode = account.AccountCode,
                    AccountName = account.AccountName,
                    Level = level,
                    Children = MapToChartOfAccountDto(account.Children, level + 1)
                };
                dtoList.Add(dto);
            }
            return dtoList;
        }
    }
} 