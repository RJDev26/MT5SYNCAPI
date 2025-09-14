using System;
using System.Threading;
using System.Threading.Tasks;
using OTS.DOMAIN.MobileAccountingVM;

namespace OTS.Service.Interfaces
{
    public interface IDealHistoryService
    {
        Task<DealHistoryResultVM> GetDealHistoryAsync(DateTime fromDate, DateTime toDate, long? managerId, long? login, CancellationToken ct);
    }
}
