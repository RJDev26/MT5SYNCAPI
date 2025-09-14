using System;
using System.Threading;
using System.Threading.Tasks;
using OTS.DOMAIN.MobileAccountingVM;

namespace MobileAccounting.Repositories.Interfaces
{
    public interface ILiveSummaryRepository
    {
        Task<LiveSummaryResultVM> GetLiveSummaryAsync(DateTime fromDate, DateTime toDate, long? managerId, string? exchange, string? option, CancellationToken ct);
    }
}
