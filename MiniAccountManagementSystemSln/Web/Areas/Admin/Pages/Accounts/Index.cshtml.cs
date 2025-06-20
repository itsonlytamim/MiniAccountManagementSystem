using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MiniAccountManagementSystemSln.Web.Areas.Admin.Pages.Accounts
{
    [Authorize(Roles = "Admin,Accountant")]
    public class IndexModel : PageModel
    {
        private readonly IAccountRepository _repo;
        public IEnumerable<Account> Accounts { get; set; } = new List<Account>();

        public IndexModel(IAccountRepository repo)
        {
            _repo = repo;
        }

        public async Task OnGetAsync()
        {
            Accounts = await _repo.GetAllHierarchicalAsync();
        }
    }
}
