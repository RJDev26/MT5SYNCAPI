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
    public class JobbingDealRepository : IJobbingDealRepository
    {
        private readonly DbManager _db;

        public JobbingDealRepository(DbManager db)
        {
            _db = db;
        }

        public async Task<JobbingDealResultVM> GetJobbingDealsAsync(DateTime? fromTime, DateTime? toTime, int intervalMinutes, long? login, string? symbol, CancellationToken ct)
        {
            var parameters = new List<DbParameter>
            {
                new DbParameter("FromTime", ParameterDirection.Input, fromTime),
                new DbParameter("ToTime", ParameterDirection.Input, toTime),
                new DbParameter("IntervalMinutes", ParameterDirection.Input, intervalMinutes),
                new DbParameter("Login", ParameterDirection.Input, login),
                new DbParameter("Symbol", ParameterDirection.Input, symbol)
            };

            var (rows, meta) = await _db.ExecuteMultipleAsync<JobbingDealVM, JobbingDealMetaVM>(
                "usp_GetJobbingPairs_ForDto", parameters);

            return new JobbingDealResultVM
            {
                Rows = rows,
                MaxTime = meta?.MaxTime,
                RowsCount = meta?.RowsCount
            };
        }
    }
}
