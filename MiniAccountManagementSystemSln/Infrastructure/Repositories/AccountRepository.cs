using Core.Entities;
using Core.Interfaces;
using Dapper;
using Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly DapperContext _context;

        public AccountRepository(DapperContext context)
        {
            _context = context;
        }

        // This helper method does NOT use LINQ for filtering/querying, only for building the hierarchy.
        private List<Account> BuildHierarchy(List<Account> allAccounts)
        {
            var lookup = new Dictionary<int, Account>();
            foreach (var acc in allAccounts)
            {
                acc.Children = new List<Account>();
                lookup.Add(acc.AccountId, acc);
            }

            var rootAccounts = new List<Account>();
            foreach (var acc in allAccounts)
            {
                if (acc.ParentAccountId.HasValue && lookup.ContainsKey(acc.ParentAccountId.Value))
                {
                    lookup[acc.ParentAccountId.Value].Children.Add(acc);
                }
                else
                {
                    rootAccounts.Add(acc);
                }
            }
            return rootAccounts;
        }

        public async Task<IEnumerable<Account>> GetAllHierarchicalAsync()
        {
            using (var connection = _context.CreateConnection())
            {
                var accounts = await connection.QueryAsync<Account>(
                    "sp_ManageChartOfAccounts",
                    new { Action = "GETALL" },
                    commandType: CommandType.StoredProcedure
                );
                // .ToList() is acceptable as it's not LINQ to SQL/Entities.
                return BuildHierarchy(accounts.ToList());
            }
        }

        public async Task<Account?> GetByIdAsync(int accountId)
        {
            using (var connection = _context.CreateConnection())
            {
                return await connection.QuerySingleOrDefaultAsync<Account>("sp_ManageChartOfAccounts",
                    new { Action = "GETBYID", AccountId = accountId },
                    commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<int> CreateAsync(Account account)
        {
            using (var connection = _context.CreateConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add("Action", "CREATE", DbType.String);
                parameters.Add("AccountCode", account.AccountCode, DbType.String);
                parameters.Add("AccountName", account.AccountName, DbType.String);
                parameters.Add("ParentAccountId", account.ParentAccountId, DbType.Int32);

                return await connection.ExecuteScalarAsync<int>("sp_ManageChartOfAccounts", parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task UpdateAsync(Account account)
        {
            using (var connection = _context.CreateConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add("Action", "UPDATE", DbType.String);
                parameters.Add("AccountId", account.AccountId, DbType.Int32);
                parameters.Add("AccountCode", account.AccountCode, DbType.String);
                parameters.Add("AccountName", account.AccountName, DbType.String);
                parameters.Add("ParentAccountId", account.ParentAccountId, DbType.Int32);

                await connection.ExecuteAsync("sp_ManageChartOfAccounts", parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task DeleteAsync(int accountId)
        {
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync("sp_ManageChartOfAccounts",
                    new { Action = "DELETE", AccountId = accountId },
                    commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<bool> HasChildrenAsync(int accountId)
        {
            using (var connection = _context.CreateConnection())
            {
                var count = await connection.ExecuteScalarAsync<int>(
                    "SELECT COUNT(1) FROM ChartOfAccounts WHERE ParentAccountId = @AccountId",
                    new { AccountId = accountId });
                return count > 0;
            }
        }
    }
}
