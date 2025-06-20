using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Pages.ChartOfAccounts
{
    [Authorize(Roles = "Admin")]
    public class DeleteModel : PageModel
    {
        private readonly IAccountRepository _accountRepo;

        public DeleteModel(IAccountRepository accountRepo)
        {
            _accountRepo = accountRepo;
        }

        [BindProperty]
        public Account Account { get; set; }
        public bool HasChildren { get; set; }
        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Account = await _accountRepo.GetByIdAsync(id);
            if (Account == null) return NotFound();
            HasChildren = await _accountRepo.HasChildrenAsync(id);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            Account = await _accountRepo.GetByIdAsync(id);
            if (Account == null) return NotFound();
            HasChildren = await _accountRepo.HasChildrenAsync(id);
            if (HasChildren)
            {
                ErrorMessage = "Cannot delete an account that has child accounts.";
                return Page();
            }
            await _accountRepo.DeleteAsync(id);
            return RedirectToPage("./Index");
        }
    }
} 