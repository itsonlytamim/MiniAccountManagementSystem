using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Web.Pages.Vouchers
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IVoucherService _voucherService;

        public IndexModel(IVoucherService voucherService)
        {
            _voucherService = voucherService;
        }

        public IEnumerable<VoucherListDto> Vouchers { get; set; }

        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 15; // Or get from config

        public async Task OnGetAsync()
        {
            Vouchers = await _voucherService.GetVouchersAsync(CurrentPage, PageSize);
        }
    }
} 