using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Web.Pages.ChartOfAccounts
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IAccountRepository _accountRepo;

        public IndexModel(IAccountRepository accountRepo)
        {
            _accountRepo = accountRepo;
        }

        public IList<Account> AccountTree { get; set; } = new List<Account>();

        public async Task OnGetAsync()
        {
            AccountTree = (List<Account>)await _accountRepo.GetAllHierarchicalAsync();
        }
    }
}