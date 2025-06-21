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
    public class CreateModel : PageModel
    {
        private readonly IAccountService _accountService;

        public CreateModel(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [BindProperty]
        public AccountDto Account { get; set; } = new AccountDto();
        
        public SelectList ParentAccountSL { get; set; }

        public async Task OnGetAsync()
        {
            await PopulateParentAccountsDropDownList();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await PopulateParentAccountsDropDownList();
                return Page();
            }

            if (Account.ParentAccountId == 0)
            {
                Account.ParentAccountId = null;
            }

            await _accountService.CreateAccountAsync(Account);

            return RedirectToPage("./Index");
        }

        private async Task PopulateParentAccountsDropDownList()
        {
            var accounts = await _accountService.GetChartOfAccountsAsync();
            var flattenedAccounts = new List<SelectListItem>();
            
            flattenedAccounts.Add(new SelectListItem { Text = "None (Top-Level Account)", Value = "0" });
            
            AddAccountsToList(accounts, flattenedAccounts, 0);

            ParentAccountSL = new SelectList(flattenedAccounts, "Value", "Text", Account.ParentAccountId);
        }
        
        private void AddAccountsToList(IEnumerable<ChartOfAccountDto> accounts, List<SelectListItem> list, int level)
        {
            foreach (var account in accounts)
            {
                list.Add(new SelectListItem
                {
                    Text = new string(' ', level * 4) + account.AccountName, // Non-breaking space for indentation
                    Value = account.AccountId.ToString()
                });
                if (account.Children.Any())
                {
                    AddAccountsToList(account.Children, list, level + 1);
                }
            }
        }
    }
}