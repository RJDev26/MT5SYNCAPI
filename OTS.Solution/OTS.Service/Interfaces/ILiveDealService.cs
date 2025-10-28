using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OTS.DOMAIN.MobileAccountingVM;

namespace OTS.Service.Interfaces
{
    public interface ILiveDealService
    {
        Task<LiveDealResultVM> GetLiveDealsAsync(DateOnly onDate, DateTime? sinceTime, string? symbol, string? action, int pageSize, bool asc, CancellationToken ct);
        Task<CrossTradePairResultVM> GetCrossTradePairsAsync(DateTime? fromTime, DateTime? toTime, CancellationToken ct);
        Task<List<CrossTradePairDiffIpVM>> GetCrossTradePairsDiffIpAsync(DateTime? fromTime, DateTime? toTime, CancellationToken ct);
    }
}
