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
        private readonly DbContext _context;

        public VoucherRepository(DbContext context)
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

        public Task<IEnumerable<Voucher>> GetAllAsync(int pageNumber, int pageSize)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<Voucher>> GetByTypeAsync(VoucherType voucherType)
        {
            throw new System.NotImplementedException();
        }
    }
}
