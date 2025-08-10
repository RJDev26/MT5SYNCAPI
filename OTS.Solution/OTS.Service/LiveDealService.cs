using System;
using System.Threading;
using System.Threading.Tasks;
using MobileAccounting.Repositories.Interfaces;
using OTS.DOMAIN.MobileAccountingVM;
using OTS.Service.Interfaces;

namespace OTS.Service
{
    public class LiveDealService : ILiveDealService
    {
        private readonly ILiveDealRepository _repository;

        public LiveDealService(ILiveDealRepository repository)
        {
            _repository = repository;
        }

        public Task<LiveDealResultVM> GetLiveDealsAsync(DateOnly onDate, DateTime? sinceTime, string? symbol, string? action, int pageSize, bool asc, CancellationToken ct)
        {
            return _repository.GetLiveDealsAsync(onDate, sinceTime, symbol, action, pageSize, asc, ct);
        }
    }
}
