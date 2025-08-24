using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MobileAccounting.Repositories.Interfaces;
using OTS.DOMAIN.MobileAccountingVM;
using OTS.Service.Interfaces;

namespace OTS.Service
{
    public class JobbingDealService : IJobbingDealService
    {
        private readonly IJobbingDealRepository _repository;

        public JobbingDealService(IJobbingDealRepository repository)
        {
            _repository = repository;
        }

        public Task<JobbingDealResultVM> GetJobbingDealsAsync(DateTime? fromTime, DateTime? toTime, int? intervalMinutes, long? login, string? symbol, CancellationToken ct)
        {
            return _repository.GetJobbingDealsAsync(fromTime, toTime, intervalMinutes ?? 5, login, symbol, ct);
        }
    }
}
