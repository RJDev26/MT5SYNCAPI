using System;
using System.Threading;
using System.Threading.Tasks;
using MobileAccounting.Repositories.Interfaces;
using OTS.DOMAIN.MobileAccountingVM;
using OTS.Service.Interfaces;

namespace OTS.Service
{
    public class DealHistoryService : IDealHistoryService
    {
        private readonly IDealHistoryRepository _repository;

        public DealHistoryService(IDealHistoryRepository repository)
        {
            _repository = repository;
        }

        public Task<DealHistoryResultVM> GetDealHistoryAsync(DateTime fromDate, DateTime toDate, long? managerId, CancellationToken ct)
        {
            return _repository.GetDealHistoryAsync(fromDate, toDate, managerId, ct);
        }
    }
}
