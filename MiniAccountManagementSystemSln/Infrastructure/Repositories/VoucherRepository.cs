using Core.Entities;
using Core.Interfaces;
using Dapper;
using Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class VoucherRepository : IVoucherRepository
    {
        private readonly DapperContext _context;

        public VoucherRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<int> CreateAsync(Voucher voucher)
        {
            // Create a DataTable in memory that matches the SQL User-Defined Table Type
            var detailsTable = new DataTable();
            detailsTable.Columns.Add("AccountId", typeof(int));
            detailsTable.Columns.Add("DebitAmount", typeof(decimal));
            detailsTable.Columns.Add("CreditAmount", typeof(decimal));

            // Populate the DataTable from the voucher's detail lines
            foreach (var detail in voucher.Details)
            {
                detailsTable.Rows.Add(detail.AccountId, detail.DebitAmount, detail.CreditAmount);
            }

            using (var connection = _context.CreateConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add("VoucherDate", voucher.VoucherDate, DbType.Date);
                parameters.Add("VoucherType", voucher.VoucherType.ToString(), DbType.String);
                parameters.Add("ReferenceNo", voucher.ReferenceNo, DbType.String);
                parameters.Add("Narration", voucher.Narration, DbType.String);
                parameters.Add("CreatedByUserId", voucher.CreatedByUserId, DbType.String);
                // Pass the DataTable as a Table-Valued Parameter
                parameters.Add("VoucherDetails", detailsTable.AsTableValuedParameter("dbo.VoucherDetailType"));

                // The SP returns the new Voucher ID
                return await connection.ExecuteScalarAsync<int>("sp_SaveVoucher", parameters, commandType: CommandType.StoredProcedure);
            }
        }

        // These methods are placeholders as per the interface.
        // Implementing them would follow a similar pattern of creating stored procedures and calling them with Dapper.
        public Task<Voucher?> GetByIdAsync(int voucherId)
        {
            throw new System.NotImplementedException();
        }

        public async Task<IEnumerable<Voucher>> GetAllAsync(int pageNumber, int pageSize)
        {
            using (var connection = _context.CreateConnection())
            {
                var sql = @"SELECT VoucherId, VoucherDate, VoucherType, ReferenceNo, Narration, CreatedByUserId
                            FROM Vouchers
                            ORDER BY VoucherDate DESC, VoucherId DESC
                            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
                return await connection.QueryAsync<Voucher>(sql, new
                {
                    Offset = (pageNumber - 1) * pageSize,
                    PageSize = pageSize
                });
            }
        }

        public Task<IEnumerable<Voucher>> GetByTypeAsync(VoucherType voucherType)
        {
            throw new System.NotImplementedException();
        }

        public async Task<Voucher?> GetWithDetailsAsync(int voucherId)
        {
            using (var connection = _context.CreateConnection())
            {
                var sql = @"
                    SELECT v.VoucherId, v.VoucherDate, v.VoucherType, v.ReferenceNo, v.Narration, v.CreatedByUserId,
                           d.VoucherDetailId, d.AccountId, d.DebitAmount, d.CreditAmount,
                           a.AccountCode, a.AccountName
                    FROM Vouchers v
                    LEFT JOIN VoucherDetails d ON v.VoucherId = d.VoucherId
                    LEFT JOIN ChartOfAccounts a ON d.AccountId = a.AccountId
                    WHERE v.VoucherId = @VoucherId
                    ORDER BY d.VoucherDetailId
                ";
                Voucher? voucher = null;
                var lookup = new Dictionary<int, Voucher>();
                var result = await connection.QueryAsync<Voucher, VoucherDetail, Account, Voucher>(
                    sql,
                    (v, d, a) =>
                    {
                        if (!lookup.TryGetValue(v.VoucherId, out voucher))
                        {
                            voucher = v;
                            voucher.Details = new List<VoucherDetail>();
                            lookup.Add(v.VoucherId, voucher);
                        }
                        if (d != null)
                        {
                            d.Account = a;
                            voucher.Details.Add(d);
                        }
                        return voucher;
                    },
                    new { VoucherId = voucherId },
                    splitOn: "VoucherDetailId,AccountCode"
                );
                return lookup.Values.FirstOrDefault();
            }
        }
    }
}
