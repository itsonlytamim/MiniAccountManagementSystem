using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace Web.Pages.ChartOfAccounts
{
    [Authorize(Roles = "Admin")]
    public class DeleteModel : PageModel
    {
        private readonly IAccountService _accountService;

        public DeleteModel(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [BindProperty]
        public AccountDto Account { get; set; }
        public string ErrorMessage { get; set; }

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

            if (!await _accountService.CanDeleteAccountAsync(id.Value))
            {
                ErrorMessage = "This account cannot be deleted because it is a parent to other accounts. Please delete or re-assign child accounts first.";
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (!await _accountService.CanDeleteAccountAsync(id.Value))
            {
                // Re-fetch data for display
                var account = await _accountService.GetAccountByIdAsync(id.Value);
                if (account == null) return NotFound();
                Account = account;
                ErrorMessage = "This account cannot be deleted because it is a parent to other accounts. Please delete or re-assign child accounts first.";
                return Page();
            }
            
            await _accountService.DeleteAccountAsync(id.Value);

            return RedirectToPage("./Index");
        }
    }
} 