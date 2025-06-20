using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Web.Pages.Vouchers
{
    [Authorize(Roles = "Admin,Accountant")]
    public class CreateModel : PageModel
    {
        private readonly IVoucherRepository _voucherRepo;
        private readonly IAccountRepository _accountRepo;

        public CreateModel(IVoucherRepository voucherRepo, IAccountRepository accountRepo)
        {
            _voucherRepo = voucherRepo;
            _accountRepo = accountRepo;
        }

        [BindProperty]
        public Voucher Voucher { get; set; } = new();

        public SelectList AccountSL { get; set; }

        private async Task PopulateAccountsDropDownList()
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
            AccountSL = new SelectList(flatList, "AccountId", "AccountName");
        }

        public async Task<IActionResult> OnGetAsync()
        {
            Voucher.VoucherDate = System.DateTime.Today;
            Voucher.Details.Add(new VoucherDetail());
            Voucher.Details.Add(new VoucherDetail());
            await PopulateAccountsDropDownList();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Voucher.Details.RemoveAll(d => d.AccountId == 0 && d.DebitAmount == 0 && d.CreditAmount == 0);

            if (!Voucher.Details.Any())
            {
                ModelState.AddModelError("Voucher.Details", "At least one voucher line is required.");
            }
            else
            {
                decimal totalDebit = Voucher.Details.Sum(d => d.DebitAmount);
                decimal totalCredit = Voucher.Details.Sum(d => d.CreditAmount);

                if (totalDebit != totalCredit)
                {
                    ModelState.AddModelError("Voucher.Details", "Total debits must equal total credits.");
                }
            }

            if (!ModelState.IsValid)
            {
                await PopulateAccountsDropDownList();
                return Page();
            }

            Voucher.CreatedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _voucherRepo.CreateAsync(Voucher);
            return RedirectToPage("/Index");
        }
    }
}