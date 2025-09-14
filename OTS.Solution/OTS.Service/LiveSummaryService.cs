using System;
using System.Threading;
using System.Threading.Tasks;
using MobileAccounting.Repositories.Interfaces;
using OTS.DOMAIN.MobileAccountingVM;
using OTS.Service.Interfaces;

namespace OTS.Service
{
    public class LiveSummaryService : ILiveSummaryService
    {
        private readonly ILiveSummaryRepository _repository;

        public LiveSummaryService(ILiveSummaryRepository repository)
        {
            _repository = repository;
        }

        public Task<LiveSummaryResultVM> GetLiveSummaryAsync(DateTime fromDate, DateTime toDate, long? managerId, string? exchange, string? option, CancellationToken ct)
        {
            return _repository.GetLiveSummaryAsync(fromDate, toDate, managerId, exchange, option, ct);
        }
    }
}
