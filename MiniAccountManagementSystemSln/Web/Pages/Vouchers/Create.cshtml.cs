using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Web.Pages.Vouchers
{
    [Authorize(Roles = "Admin,Accountant")]
    public class CreateModel : PageModel
    {
        private readonly IVoucherService _voucherService;
        private readonly IAccountService _accountService; // For populating accounts dropdown

        public CreateModel(IVoucherService voucherService, IAccountService accountService)
        {
            _voucherService = voucherService;
            _accountService = accountService;
        }

        [BindProperty]
        public VoucherCreateDto Voucher { get; set; } = new VoucherCreateDto();

        public SelectList AccountSL { get; set; }

        public async Task OnGetAsync()
        {
            await PopulateAccountsDropDownList();
            // Initialize with one empty row for data entry
            Voucher.Details.Add(new VoucherDetailCreateDto());
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Clean up empty rows submitted by the form
            Voucher.Details.RemoveAll(d => d.AccountId == 0 && d.DebitAmount == 0 && d.CreditAmount == 0);

            // Basic business rule validation
            if (!Voucher.Details.Any())
            {
                ModelState.AddModelError(string.Empty, "Voucher must have at least one detail line.");
            }
            else
            {
                decimal totalDebit = Voucher.Details.Sum(d => d.DebitAmount);
                decimal totalCredit = Voucher.Details.Sum(d => d.CreditAmount);

                if (totalDebit != totalCredit)
                {
                    ModelState.AddModelError(string.Empty, "Total debit and credit amounts must be equal.");
                }

                if (totalDebit == 0)
                {
                    ModelState.AddModelError(string.Empty, "Voucher total cannot be zero.");
                }
            }
            
            if (!ModelState.IsValid)
            {
                await PopulateAccountsDropDownList();
                return Page();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var newVoucherId = await _voucherService.CreateVoucherAsync(Voucher, userId);

            return RedirectToPage("./Details", new { id = newVoucherId });
        }
        
        private async Task PopulateAccountsDropDownList()
        {
            var accounts = await _accountService.GetChartOfAccountsAsync();
            var flattenedAccounts = new List<SelectListItem>();
            AddAccountsToList(accounts, flattenedAccounts, 0);
            AccountSL = new SelectList(flattenedAccounts, "Value", "Text");
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