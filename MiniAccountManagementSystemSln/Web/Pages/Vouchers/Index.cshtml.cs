using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Web.Pages.Vouchers
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IVoucherRepository _voucherRepo;
        public IndexModel(IVoucherRepository voucherRepo)
        {
            _voucherRepo = voucherRepo;
        }
        public IList<Voucher> Vouchers { get; set; } = new List<Voucher>();
        public async Task OnGetAsync()
        {
            // For demo, get first 100 vouchers
            Vouchers = (IList<Voucher>)await _voucherRepo.GetAllAsync(1, 100);
        }
    }
} 