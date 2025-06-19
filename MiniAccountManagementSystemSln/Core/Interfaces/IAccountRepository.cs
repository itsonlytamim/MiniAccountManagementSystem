using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IAccountRepository
    {
        Task<IEnumerable<Account>> GetAllHierarchicalAsync();
        Task<int> CreateAsync(Account account);
        Task UpdateAsync(Account account);
        Task DeleteAsync(int accountId);
        Task<Account?> GetByIdAsync(int accountId);
    }
}
