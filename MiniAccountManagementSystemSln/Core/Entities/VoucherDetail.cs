using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class VoucherDetail
    {
        public int VoucherDetailId { get; set; }
        public int VoucherId { get; set; }
        public int AccountId { get; set; }
        public decimal DebitAmount { get; set; }
        public decimal CreditAmount { get; set; }
        public Account? Account { get; set; }
    }
}
