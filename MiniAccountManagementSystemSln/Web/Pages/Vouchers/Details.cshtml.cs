using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace Web.Pages.Vouchers
{
    [Authorize]
    public class DetailsModel : PageModel
    {
        private readonly IVoucherRepository _voucherRepo;
        public DetailsModel(IVoucherRepository voucherRepo)
        {
            _voucherRepo = voucherRepo;
        }
        public Voucher Voucher { get; set; }
        public async Task<IActionResult> OnGetAsync(int id)
        {
            Voucher = await _voucherRepo.GetWithDetailsAsync(id);
            if (Voucher == null) return NotFound();
            return Page();
        }
    }
} 