using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MobileAccounting.Repositories.Interfaces;
using OTS.DOMAIN.MobileAccountingVM;
using OTS.Infrastructutre.Generic.WebBroker.DataAccessCore;

namespace MobileAccounting.Repositories.Implementations
{
    public class LiveDealRepository : ILiveDealRepository
    {
        private readonly DbManager _db;

        public LiveDealRepository(DbManager db)
        {
            _db = db;
        }

        public async Task<LiveDealResultVM> GetLiveDealsAsync(DateOnly onDate, DateTime? sinceTime, string? symbol, string? action, int pageSize, bool asc, CancellationToken ct)
        {
            var parameters = new List<DbParameter>
            {
                new DbParameter("OnDate", ParameterDirection.Input, onDate.ToDateTime(TimeOnly.MinValue)),
                new DbParameter("SinceTime", ParameterDirection.Input, sinceTime),
                new DbParameter("Symbol", ParameterDirection.Input, symbol),
                new DbParameter("Action", ParameterDirection.Input, action),
                new DbParameter("PageSize", ParameterDirection.Input, pageSize)
            };

            var (rows, meta) = await _db.ExecuteMultipleAsync<LiveDealVM, LiveDealMetaVM>("usp_GetLiveDeals", parameters);

            if (!asc)
            {
                rows = rows.OrderByDescending(r => r.Time).ToList();
            }

            return new LiveDealResultVM
            {
                Rows = rows,
                MaxTime = meta?.MaxTime,
                TotalRows = meta?.TotalRows
            };
        }
    }
}
