using System;
using System.Threading;
using System.Threading.Tasks;
using OTS.DOMAIN.MobileAccountingVM;

namespace OTS.Service.Interfaces
{
    public interface ILiveSummaryService
    {
        Task<LiveSummaryResultVM> GetLiveSummaryAsync(DateTime fromDate, DateTime toDate, long? managerId, string? exchange, CancellationToken ct);
    }
}
