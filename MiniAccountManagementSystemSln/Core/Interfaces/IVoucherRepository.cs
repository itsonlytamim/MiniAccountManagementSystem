using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IVoucherRepository
    {
        Task<int> CreateAsync(Voucher voucher);
        Task<Voucher?> GetByIdAsync(int voucherId);
        Task<IEnumerable<Voucher>> GetAllAsync(int pageNumber, int pageSize);
        Task<IEnumerable<Voucher>> GetByTypeAsync(VoucherType voucherType);
    }
}
