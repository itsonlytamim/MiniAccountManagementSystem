using Application.DTOs;
using Application.Interfaces;
using Core.Entities;
using System.Drawing;
using System.Security.Claims;

namespace Application.Services
{
    public class VoucherService : IVoucherService
    {
        private readonly IVoucherRepository _voucherRepository;

        public VoucherService(IVoucherRepository voucherRepository)
        {
            _voucherRepository = voucherRepository;
        }

        public async Task<int> CreateVoucherAsync(VoucherCreateDto voucherDto, string userId)
        {
            var voucher = new Voucher
            {
                VoucherDate = voucherDto.VoucherDate,
                VoucherType = Enum.Parse<VoucherType>(voucherDto.VoucherType),
                ReferenceNo = voucherDto.ReferenceNo,
                Narration = voucherDto.Narration,
                CreatedByUserId = userId,
                Details = voucherDto.Details.Select(d => new VoucherDetail
                {
                    AccountId = d.AccountId,
                    DebitAmount = d.DebitAmount,
                    CreditAmount = d.CreditAmount
                }).ToList()
            };

            return await _voucherRepository.CreateAsync(voucher);
        }

        public async Task<IEnumerable<VoucherListDto>> GetVouchersAsync(int pageNumber, int pageSize)
        {
            var vouchers = await _voucherRepository.GetAllAsync(pageNumber, pageSize);
            return vouchers.Select(v => new VoucherListDto
            {
                VoucherId = v.VoucherId,
                VoucherDate = v.VoucherDate,
                VoucherType = v.VoucherType.ToString(),
                ReferenceNo = v.ReferenceNo,
                Narration = v.Narration
            });
        }
        
        public async Task<IEnumerable<VoucherListDto>> GetAllVouchersForExportAsync()
        {
            var vouchers = await _voucherRepository.GetAllForExportAsync();
            return vouchers.Select(v => new VoucherListDto
            {
                VoucherId = v.VoucherId,
                VoucherDate = v.VoucherDate,
                VoucherType = v.VoucherType.ToString(),
                ReferenceNo = v.ReferenceNo,
                Narration = v.Narration
            });
        }

        public async Task<VoucherDetailViewDto?> GetVoucherDetailsAsync(int voucherId)
        {
            var voucher = await _voucherRepository.GetWithDetailsAsync(voucherId);
            if (voucher == null) return null;

            return new VoucherDetailViewDto
            {
                VoucherId = voucher.VoucherId,
                VoucherDate = voucher.VoucherDate,
                VoucherType = voucher.VoucherType.ToString(),
                ReferenceNo = voucher.ReferenceNo,
                Narration = voucher.Narration,
                CreatedByUserId = voucher.CreatedByUserId,
                Details = voucher.Details.Select(d => new VoucherDetailItemDto
                {
                    AccountId = d.AccountId,
                    AccountCode = d.Account.AccountCode,
                    AccountName = d.Account.AccountName,
                    DebitAmount = d.DebitAmount,
                    CreditAmount = d.CreditAmount
                }).ToList(),
                TotalDebit = voucher.Details.Sum(d => d.DebitAmount),
                TotalCredit = voucher.Details.Sum(d => d.CreditAmount)
            };
        }
    }
} 