namespace Application.DTOs
{
    public class AccountDto
    {
        public int AccountId { get; set; }
        public string AccountCode { get; set; } = string.Empty;
        public string AccountName { get; set; } = string.Empty;
        public int? ParentAccountId { get; set; }
    }

    public class ChartOfAccountDto
    {
        public int AccountId { get; set; }
        public string AccountCode { get; set; } = string.Empty;
        public string AccountName { get; set; } = string.Empty;
        public int Level { get; set; } // For indentation in the view
        public List<ChartOfAccountDto> Children { get; set; } = new List<ChartOfAccountDto>();
    }
} 