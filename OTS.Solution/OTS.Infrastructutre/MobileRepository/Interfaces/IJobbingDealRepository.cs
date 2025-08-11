using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OTS.DOMAIN.MobileAccountingVM;

namespace MobileAccounting.Repositories.Interfaces
{
    public interface IJobbingDealRepository
    {
        Task<List<JobbingDealVM>> GetJobbingDealsAsync(DateTime? fromTime, DateTime? toTime, int intervalMinutes, long? login, string? symbol, CancellationToken ct);
    }
}
