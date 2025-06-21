using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Web.Pages.ChartOfAccounts
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IAccountService _accountService;

        public IndexModel(IAccountService accountService)
        {
            _accountService = accountService;
        }

        public List<ChartOfAccountDto> Accounts { get; set; } = new List<ChartOfAccountDto>();

        public async Task OnGetAsync()
        {
            Accounts = (await _accountService.GetChartOfAccountsAsync()).ToList();
        }
    }
}