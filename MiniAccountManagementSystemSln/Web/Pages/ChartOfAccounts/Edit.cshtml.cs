using Application.DTOs;
using Application.Interfaces;
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
        private readonly IAccountService _accountService;

        public EditModel(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [BindProperty]
        public AccountDto Account { get; set; }
        public SelectList ParentAccountSL { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _accountService.GetAccountByIdAsync(id.Value);
            if (account == null)
            {
                return NotFound();
            }
            Account = account;

            await PopulateParentAccountsDropDownList(Account.AccountId);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await PopulateParentAccountsDropDownList(Account.AccountId);
                return Page();
            }
            
            if (Account.ParentAccountId == 0)
            {
                Account.ParentAccountId = null;
            }
            
            // Basic circular dependency check
            if (Account.ParentAccountId == Account.AccountId)
            {
                ModelState.AddModelError("Account.ParentAccountId", "An account cannot be its own parent.");
                await PopulateParentAccountsDropDownList(Account.AccountId);
                return Page();
            }

            await _accountService.UpdateAccountAsync(Account);

            return RedirectToPage("./Index");
        }

        private async Task PopulateParentAccountsDropDownList(int currentAccountId)
        {
            var accounts = await _accountService.GetChartOfAccountsAsync();
            var flattenedAccounts = new List<SelectListItem>();
            flattenedAccounts.Add(new SelectListItem { Text = "None (Top-Level Account)", Value = "0" });
            
            AddAccountsToList(accounts, flattenedAccounts, 0, currentAccountId);

            ParentAccountSL = new SelectList(flattenedAccounts, "Value", "Text", Account.ParentAccountId);
        }

        private void AddAccountsToList(IEnumerable<ChartOfAccountDto> accounts, List<SelectListItem> list, int level, int currentAccountId)
        {
            foreach (var account in accounts)
            {
                // Exclude the current account and its descendants from the list of potential parents
                if (account.AccountId == currentAccountId) continue;

                list.Add(new SelectListItem
                {
                    Text = new string(' ', level * 4) + account.AccountName,
                    Value = account.AccountId.ToString()
                });

                if (account.Children.Any())
                {
                    AddAccountsToList(account.Children, list, level + 1, currentAccountId);
                }
            }
        }
    }
} 