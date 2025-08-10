using OTS.DOMAIN.MobileAccountingVM;
using OTS.DOMAIN.MobileAccoutingDTO;

namespace OTS.Service.Interfaces
{
    public interface IVoucherService
    {
        Task<string> SaveOrUpdateVoucherAsync(VoucherEntryRequest request);
        Task<List<VoucherSummaryVM>> GetVoucherSummariesAsync(VoucherSummaryRequest request);
        Task<VoucherDetailVM?> GetVoucherByIdAsync(int voucherId);
        Task<string> DeleteVoucherAsync(DeleteVoucherRequest request);
        Task<List<LedgerEntryVM>> GetLedgerAsync(LedgerRequest request);
        Task<TrialBalanceResultDto> GetTrialBalanceAsync(TrialBalanceRequest request);
    }

}
