using MobileAccounting.Repositories.Implementations;
using MobileAccounting.Repositories.Interfaces;
using OTS.DOMAIN.MobileAccountingVM;
using OTS.DOMAIN.MobileAccoutingDTO;
using OTS.Service.Interfaces;

namespace OTS.Service
{
    public class VoucherService : IVoucherService
    {
        private readonly IVoucherDetailRepository _repository;

        public VoucherService(IVoucherDetailRepository repository)
        {
            _repository = repository;
        }

        public async Task<string> SaveOrUpdateVoucherAsync(VoucherEntryRequest request)
        {
            return await _repository.SaveOrUpdateVoucherAsync(request);
        }
        public async Task<List<VoucherSummaryVM>> GetVoucherSummariesAsync(VoucherSummaryRequest request)
        {
            return await _repository.GetVoucherSummariesAsync(request);
        }
        public Task<VoucherDetailVM?> GetVoucherByIdAsync(int voucherId)
        {
            return _repository.GetVoucherByIdAsync(voucherId);
        }
        public async Task<string> DeleteVoucherAsync(DeleteVoucherRequest request)
        {
            return await _repository.DeleteVoucherAsync(request.VoucherId, request.UserId);
        }
        public async Task<List<LedgerEntryVM>> GetLedgerAsync(LedgerRequest request)
        {
            return await _repository.GetLedgerReportAsync(request);
        }
        public Task<TrialBalanceResultDto> GetTrialBalanceAsync(TrialBalanceRequest request)
        {
            return _repository.GetTrialBalanceAsync(request);
        }
       
    }

}
