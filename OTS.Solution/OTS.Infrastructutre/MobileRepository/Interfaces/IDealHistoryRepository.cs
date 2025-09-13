using System;
using System.Threading;
using System.Threading.Tasks;
using OTS.DOMAIN.MobileAccountingVM;

namespace MobileAccounting.Repositories.Interfaces
{
    public interface IDealHistoryRepository
    {
        Task<DealHistoryResultVM> GetDealHistoryAsync(DateTime fromDate, DateTime toDate, long? managerId, CancellationToken ct);
    }
}
