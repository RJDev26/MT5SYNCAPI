using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OTS.DOMAIN.MobileAccountingVM;

namespace OTS.Service.Interfaces
{
    public interface ILiveDealService
    {
        /// <summary>
        /// Retrieves paged live deals for the supplied filters and restricts the result set to the specified user.
        /// </summary>
        /// <param name="onDate">Trading date to evaluate.</param>
        /// <param name="sinceTime">Optional cursor for incremental fetches.</param>
        /// <param name="symbol">Optional symbol filter.</param>
        /// <param name="action">Optional action filter.</param>
        /// <param name="pageSize">Number of rows to return.</param>
        /// <param name="asc">Whether rows should be sorted ascending.</param>
        /// <param name="userId">The user whose data should be returned.</param>
        /// <param name="ct">Cancellation token.</param>
        Task<LiveDealResultVM> GetLiveDealsAsync(DateOnly onDate, DateTime? sinceTime, string? symbol, string? action, int pageSize, bool asc, int userId, CancellationToken ct);
        Task<CrossTradePairResultVM> GetCrossTradePairsAsync(DateTime? fromTime, DateTime? toTime, CancellationToken ct);
        Task<List<CrossTradePairDiffIpVM>> GetCrossTradePairsDiffIpAsync(DateTime? fromTime, DateTime? toTime, CancellationToken ct);
    }
}
