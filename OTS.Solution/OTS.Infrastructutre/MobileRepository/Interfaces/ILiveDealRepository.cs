using System;
using System.Threading;
using System.Threading.Tasks;
using OTS.DOMAIN.MobileAccountingVM;

namespace MobileAccounting.Repositories.Interfaces
{
    public interface ILiveDealRepository
    {
        Task<LiveDealResultVM> GetLiveDealsAsync(DateOnly onDate, DateTime? sinceTime, string? symbol, string? action, int pageSize, bool asc, CancellationToken ct);
    }
}
