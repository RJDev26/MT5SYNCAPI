using MobileAccounting.Entities;
using MobileAccounting.Repositories.Interfaces;
using OTS.DOMAIN.Database;
using OTS.DOMAIN.MobileAccountingVM;
using OTS.DOMAIN.MobileAccoutingDTO;
using OTS.Infrastructutre.Generic.WebBroker.DataAccessCore;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace MobileAccounting.Repositories.Implementations
{
    public class VoucherDetailRepository : IVoucherDetailRepository
    {
        private readonly AccountingDbContext _context;
        private readonly DbManager _db;

        public VoucherDetailRepository(DbManager db, AccountingDbContext context)
        {
            _db = db;
            _context = context;
        }

        public async Task<string> SaveOrUpdateVoucherAsync(VoucherEntryRequest request)
        {
            var parameters = new List<DbParameter>
    {
        new DbParameter("VoucherId", ParameterDirection.InputOutput, request.VoucherId ?? 0),
        new DbParameter("VoucherDate", ParameterDirection.Input, request.VoucherDate),
        new DbParameter("VoucherType", ParameterDirection.Input, request.VoucherType),
        new DbParameter("CreatedBy", ParameterDirection.Input, request.CreatedBy),
        new DbParameter("DrAccountId", ParameterDirection.Input, request.DrAccountId),
        new DbParameter("CrAccountId", ParameterDirection.Input, request.CrAccountId),
        new DbParameter("Amount", ParameterDirection.Input, request.Amount),
        new DbParameter("Narration", ParameterDirection.Input, request.Narration),
        new DbParameter("Message", ParameterDirection.Output, null, 200)
    };

            await _db.ExecuteNonQueryAsync("usp_SaveOrUpdate_VoucherEntry", parameters);

            // Read OUT params
            request.VoucherId = Convert.ToInt32(parameters.First(p => p.Name == "VoucherId").Value);
            var message = parameters.First(p => p.Name == "Message").Value?.ToString();

            return message ?? "No message returned";
        }







        public async Task<List<VoucherSummaryVM>> GetVoucherSummariesAsync(VoucherSummaryRequest request)
        {
            var parameters = new List<DbParameter>
    {
        new DbParameter("VoucherType", ParameterDirection.Input, request.VoucherType),
        new DbParameter("FromDate", ParameterDirection.Input, request.FromDate),
        new DbParameter("ToDate", ParameterDirection.Input, request.ToDate),
        new DbParameter("CreatedBy", ParameterDirection.Input, request.CreatedBy)
    };

            return await _db.ExecuteListAsync<VoucherSummaryVM>("usp_GetVoucherList_Summary", parameters);
        }


        public async Task<VoucherDetailVM?> GetVoucherByIdAsync(int voucherId)
        {
            var parameters = new List<DbParameter>
        {
            new DbParameter("VoucherId", ParameterDirection.Input, voucherId)
        };

            var result = await _db.ExecuteListAsync<VoucherDetailVM>("usp_GetVoucherById", parameters);
            return result.FirstOrDefault();
        }
        public async Task<string> DeleteVoucherAsync(int voucherId, int userId)
        {
            var parameters = new List<DbParameter>
        {
            new DbParameter("VoucherId", ParameterDirection.Input, voucherId),
            new DbParameter("UserId", ParameterDirection.Input, userId),
            new DbParameter("Message", ParameterDirection.Output, null, 255)
        };

            await _db.ExecuteNonQueryAsync("usp_Delete_Voucher", parameters);

            var message = parameters.First(p => p.Name == "Message").Value?.ToString();
            return message ?? "No message returned";
        }

        public async Task<List<LedgerEntryVM>> GetLedgerReportAsync(LedgerRequest request)
        {
            var parameters = new List<DbParameter>
    {
        new DbParameter("FromDate", ParameterDirection.Input, request.FromDate),
        new DbParameter("ToDate", ParameterDirection.Input, request.ToDate),
        new DbParameter("AccountId", ParameterDirection.Input, request.AccountId)
    };

            var result = await _db.ExecuteListAsync<LedgerEntryVM>("usp_GetLedgerReport", parameters);
            return result;
        }

        public async Task<TrialBalanceResultDto> GetTrialBalanceAsync(TrialBalanceRequest request)
        {
            var parameters = new List<DbParameter>
    {
        new DbParameter("FromDate", ParameterDirection.Input, request.FromDate),
        new DbParameter("ToDate", ParameterDirection.Input, request.ToDate),
        new DbParameter("IncludeOpBal", ParameterDirection.Input, request.IncludeOpBal),
        new DbParameter("GroupId", ParameterDirection.Input, request.GroupId)
    };

            var (rows, summary) = await _db.ExecuteMultipleAsync<TrialBalanceRowDto, TrialBalanceSummaryDto>(
                "usp_Get_TrialBalance", parameters);

            return new TrialBalanceResultDto
            {
                Rows = rows,
                Summary = summary
            };
        }

    }
}