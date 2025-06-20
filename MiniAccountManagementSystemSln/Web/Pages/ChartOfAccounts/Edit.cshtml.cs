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
    public class EditModel : PageModel
    {
        private readonly IAccountRepository _accountRepo;

        public EditModel(IAccountRepository accountRepo)
        {
            _accountRepo = accountRepo;
        }

        [BindProperty]
        public Account Account { get; set; }

        public SelectList ParentAccountSL { get; set; }

        private async Task PopulateParentAccountsDropDownList(int? excludeId = null)
        {
            var allAccounts = await _accountRepo.GetAllHierarchicalAsync();
            var flatList = new List<Account>();

            void Flatten(IEnumerable<Account> accounts, int level = 0)
            {
                foreach (var acc in accounts)
                {
                    if (excludeId.HasValue && acc.AccountId == excludeId.Value) continue;
                    acc.AccountName = new string(' ', level * 4) + acc.AccountName;
                    flatList.Add(acc);
                    if (acc.Children.Any()) Flatten(acc.Children, level + 1);
                }
            }
            Flatten(allAccounts);

            ParentAccountSL = new SelectList(flatList, "AccountId", "AccountName");
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Account = await _accountRepo.GetByIdAsync(id);
            if (Account == null) return NotFound();
            await PopulateParentAccountsDropDownList(id);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await PopulateParentAccountsDropDownList(Account.AccountId);
                return Page();
            }
            await _accountRepo.UpdateAsync(Account);
            return RedirectToPage("./Index");
        }
    }
} 