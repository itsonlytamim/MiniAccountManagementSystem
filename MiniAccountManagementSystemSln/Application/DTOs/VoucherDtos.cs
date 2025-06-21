using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    // For creating new vouchers
    public class VoucherCreateDto
    {
        [Required]
        public DateTime VoucherDate { get; set; } = DateTime.Today;

        [Required]
        public string VoucherType { get; set; }

        public string? ReferenceNo { get; set; }

        [Required]
        [StringLength(500)]
        public string Narration { get; set; }

        public List<VoucherDetailCreateDto> Details { get; set; } = new List<VoucherDetailCreateDto>();
    }

    public class VoucherDetailCreateDto
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please select an account.")]
        public int AccountId { get; set; }

        [Range(0, double.MaxValue)]
        public decimal DebitAmount { get; set; }

        [Range(0, double.MaxValue)]
        public decimal CreditAmount { get; set; }
    }

    // For displaying voucher lists
    public class VoucherListDto
    {
        public int VoucherId { get; set; }
        public DateTime VoucherDate { get; set; }
        public string VoucherType { get; set; }
        public string? ReferenceNo { get; set; }
        public string Narration { get; set; }
    }

    // For displaying voucher details
    public class VoucherDetailViewDto
    {
        public int VoucherId { get; set; }
        public DateTime VoucherDate { get; set; }
        public string VoucherType { get; set; }
        public string? ReferenceNo { get; set; }
        public string Narration { get; set; }
        public string CreatedByUserId { get; set; }
        public List<VoucherDetailItemDto> Details { get; set; } = new List<VoucherDetailItemDto>();
        public decimal TotalDebit { get; set; }
        public decimal TotalCredit { get; set; }
    }

    public class VoucherDetailItemDto
    {
        public int AccountId { get; set; }
        public string AccountCode { get; set; }
        public string AccountName { get; set; }
        public decimal DebitAmount { get; set; }
        public decimal CreditAmount { get; set; }
    }
} 