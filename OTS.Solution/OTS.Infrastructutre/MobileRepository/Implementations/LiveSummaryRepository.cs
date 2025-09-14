using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using MobileAccounting.Repositories.Interfaces;
using OTS.DOMAIN.MobileAccountingVM;
using OTS.Infrastructutre.Generic.WebBroker.DataAccessCore;

namespace MobileAccounting.Repositories.Implementations
{
    public class LiveSummaryRepository : ILiveSummaryRepository
    {
        private readonly DbManager _db;

        public LiveSummaryRepository(DbManager db)
        {
            _db = db;
        }

        public async Task<LiveSummaryResultVM> GetLiveSummaryAsync(DateTime fromDate, DateTime toDate, long? managerId, string? exchange, string? option, CancellationToken ct)
        {
            var parameters = new List<DbParameter>
            {
                new DbParameter("FromDate", ParameterDirection.Input, fromDate),
                new DbParameter("ToDate", ParameterDirection.Input, toDate),
                new DbParameter("ManagerId", ParameterDirection.Input, managerId),
                new DbParameter("Exchange", ParameterDirection.Input, exchange),
                new DbParameter("Option", ParameterDirection.Input, option)
            };

            var rows = await _db.ExecuteListAsync<LiveSummaryVM>("usp_GetLiveSummary", parameters);
            return new LiveSummaryResultVM
            {
                Rows = rows,
                RowCount = rows.Count
            };
        }
    }
}
