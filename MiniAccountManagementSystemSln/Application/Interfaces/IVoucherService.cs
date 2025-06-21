using Application.DTOs;

namespace Application.Interfaces
{
    public interface IVoucherService
    {
        Task<int> CreateVoucherAsync(VoucherCreateDto voucherDto, string userId);
        Task<IEnumerable<VoucherListDto>> GetVouchersAsync(int pageNumber, int pageSize);
        Task<VoucherDetailViewDto?> GetVoucherDetailsAsync(int voucherId);
    }
} 