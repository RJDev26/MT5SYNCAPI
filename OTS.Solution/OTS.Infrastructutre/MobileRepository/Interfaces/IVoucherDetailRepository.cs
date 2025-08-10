using MobileAccounting.Entities;
using OTS.DOMAIN.MobileAccountingVM;
using OTS.DOMAIN.MobileAccoutingDTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MobileAccounting.Repositories.Interfaces
{
    public interface IVoucherDetailRepository
    {
        Task<string> SaveOrUpdateVoucherAsync(VoucherEntryRequest request);
        Task<List<VoucherSummaryVM>> GetVoucherSummariesAsync(VoucherSummaryRequest request);
        Task<VoucherDetailVM?> GetVoucherByIdAsync(int voucherId);
        Task<string> DeleteVoucherAsync(int voucherId, int userId);
        Task<List<LedgerEntryVM>> GetLedgerReportAsync(LedgerRequest request);
        Task<TrialBalanceResultDto> GetTrialBalanceAsync(TrialBalanceRequest request);

    }
}