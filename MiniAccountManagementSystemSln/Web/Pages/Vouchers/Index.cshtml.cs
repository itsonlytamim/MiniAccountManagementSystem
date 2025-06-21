using Application.DTOs;
using Application.Interfaces;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Web.Pages.Vouchers
{
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
        public int PageSize { get; set; } = 15;

        public async Task OnGetAsync()
        {
            Vouchers = await _voucherService.GetVouchersAsync(CurrentPage, PageSize);
        }

        public async Task<IActionResult> OnGetExportAsync()
        {
            var vouchers = await _voucherService.GetAllVouchersForExportAsync();
            
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Vouchers");
            
            var currentRow = 1;
            worksheet.Cell(currentRow, 1).Value = "Voucher ID";
            worksheet.Cell(currentRow, 2).Value = "Date";
            worksheet.Cell(currentRow, 3).Value = "Type";
            worksheet.Cell(currentRow, 4).Value = "Reference No";
            worksheet.Cell(currentRow, 5).Value = "Narration";
            
            // Style header
            worksheet.Row(currentRow).Style.Font.Bold = true;
            worksheet.Row(currentRow).Style.Fill.BackgroundColor = XLColor.LightGray;

            foreach (var voucher in vouchers)
            {
                currentRow++;
                worksheet.Cell(currentRow, 1).Value = voucher.VoucherId;
                worksheet.Cell(currentRow, 2).Value = voucher.VoucherDate;
                worksheet.Cell(currentRow, 3).Value = voucher.VoucherType;
                worksheet.Cell(currentRow, 4).Value = voucher.ReferenceNo;
                worksheet.Cell(currentRow, 5).Value = voucher.Narration;
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var content = stream.ToArray();
            
            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Vouchers_{System.DateTime.Now:yyyy-MM-dd}.xlsx");
        }

        public async Task<IActionResult> OnGetExportSingleAsync(int id)
        {
            var voucher = await _voucherService.GetVoucherDetailsAsync(id);

            if (voucher == null)
            {
                return NotFound();
            }

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add($"Voucher-{voucher.VoucherId}");

            // --- Header Section ---
            worksheet.Cell("A1").Value = "Voucher Details";
            worksheet.Range("A1:B1").Merge().Style.Font.Bold = true;
            worksheet.Range("A1:B1").Style.Font.FontSize = 16;
            worksheet.Range("A1:B1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            worksheet.Cell("A3").Value = "Voucher ID:";
            worksheet.Cell("B3").Value = voucher.VoucherId;
            worksheet.Cell("A4").Value = "Date:";
            worksheet.Cell("B4").Value = voucher.VoucherDate.ToShortDateString();
            worksheet.Cell("A5").Value = "Type:";
            worksheet.Cell("B5").Value = voucher.VoucherType;
            worksheet.Cell("A6").Value = "Reference No:";
            worksheet.Cell("B6").Value = voucher.ReferenceNo;
            worksheet.Cell("A7").Value = "Narration:";
            worksheet.Cell("B7").Value = voucher.Narration;
            
            worksheet.Range("A3:A7").Style.Font.Bold = true;
            
            // --- Details Table ---
            var currentRow = 9;
            worksheet.Cell(currentRow, 1).Value = "Account Code";
            worksheet.Cell(currentRow, 2).Value = "Account Name";
            worksheet.Cell(currentRow, 3).Value = "Debit";
            worksheet.Cell(currentRow, 4).Value = "Credit";

            worksheet.Row(currentRow).Style.Font.Bold = true;
            worksheet.Row(currentRow).Style.Fill.BackgroundColor = XLColor.LightGray;

            foreach (var detail in voucher.Details)
            {
                currentRow++;
                worksheet.Cell(currentRow, 1).Value = detail.AccountCode;
                worksheet.Cell(currentRow, 2).Value = detail.AccountName;
                worksheet.Cell(currentRow, 3).Value = detail.DebitAmount;
                worksheet.Cell(currentRow, 4).Value = detail.CreditAmount;
                worksheet.Cell(currentRow, 3).Style.NumberFormat.Format = "#,##0.00";
                worksheet.Cell(currentRow, 4).Style.NumberFormat.Format = "#,##0.00";
            }
            
            // --- Footer / Totals ---
            currentRow++;
            worksheet.Cell(currentRow, 2).Value = "Total";
            worksheet.Cell(currentRow, 3).Value = voucher.TotalDebit;
            worksheet.Cell(currentRow, 4).Value = voucher.TotalCredit;
            worksheet.Cell(currentRow, 3).Style.NumberFormat.Format = "#,##0.00";
            worksheet.Cell(currentRow, 4).Style.NumberFormat.Format = "#,##0.00";
            var totalRowRange = worksheet.Range(currentRow, 2, currentRow, 4);
            totalRowRange.Style.Font.Bold = true;
            totalRowRange.Style.Border.SetTopBorder(XLBorderStyleValues.Thin);

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var content = stream.ToArray();
            
            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Voucher_{voucher.VoucherId}.xlsx");
        }
    }
} 