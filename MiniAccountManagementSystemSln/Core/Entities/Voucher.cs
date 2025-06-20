using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Voucher
    {
        public int VoucherId { get; set; }
        public DateTime VoucherDate { get; set; }
        public VoucherType VoucherType { get; set; }
        public string? ReferenceNo { get; set; }
        public string? Narration { get; set; }
        public string? CreatedByUserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<VoucherDetail> Details { get; set; } = new List<VoucherDetail>();
    }
}
