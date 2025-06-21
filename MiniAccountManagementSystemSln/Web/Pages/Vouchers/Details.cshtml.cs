using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace Web.Pages.Vouchers
{
    [Authorize]
    public class DetailsModel : PageModel
    {
        private readonly IVoucherService _voucherService;

        public DetailsModel(IVoucherService voucherService)
        {
            _voucherService = voucherService;
        }

        public VoucherDetailViewDto Voucher { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Voucher = await _voucherService.GetVoucherDetailsAsync(id.Value);

            if (Voucher == null)
            {
                return NotFound();
            }

            return Page();
        }
    }
} 