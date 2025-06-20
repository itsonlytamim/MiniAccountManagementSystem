using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Pages.ChartOfAccounts
{
    [Authorize(Roles = "Admin,Accountant")]
    public class CreateModel : PageModel
    {
        private readonly IAccountRepository _accountRepo;

        public CreateModel(IAccountRepository accountRepo)
        {
            _accountRepo = accountRepo;
        }

        [BindProperty]
        public Account Account { get; set; }

        public SelectList ParentAccountSL { get; set; }

        private async Task PopulateParentAccountsDropDownList()
        {
            var allAccounts = await _accountRepo.GetAllHierarchicalAsync();
            var flatList = new List<Account>();

            void Flatten(IEnumerable<Account> accounts, int level = 0)
            {
                foreach (var acc in accounts)
                {
                    acc.AccountName = new string(' ', level * 4) + acc.AccountName;
                    flatList.Add(acc);
                    if (acc.Children.Any()) Flatten(acc.Children, level + 1);
                }
            }
            Flatten(allAccounts);

            ParentAccountSL = new SelectList(flatList, "AccountId", "AccountName");
        }

        public async Task<IActionResult> OnGetAsync()
        {
            await PopulateParentAccountsDropDownList();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await PopulateParentAccountsDropDownList();
                return Page();
            }

            await _accountRepo.CreateAsync(Account);
            return RedirectToPage("./Index");
        }
    }
}