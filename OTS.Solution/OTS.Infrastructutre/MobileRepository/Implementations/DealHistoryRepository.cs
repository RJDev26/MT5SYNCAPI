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
    public class DealHistoryRepository : IDealHistoryRepository
    {
        private readonly DbManager _db;

        public DealHistoryRepository(DbManager db)
        {
            _db = db;
        }

        public async Task<DealHistoryResultVM> GetDealHistoryAsync(DateTime fromDate, DateTime toDate, long? managerId, CancellationToken ct)
        {
            var parameters = new List<DbParameter>
            {
                new DbParameter("FromDate", ParameterDirection.Input, fromDate),
                new DbParameter("ToDate", ParameterDirection.Input, toDate),
                new DbParameter("ManagerId", ParameterDirection.Input, managerId)
            };

            var rows = await _db.ExecuteListAsync<DealHistoryVM>("usp_GetDealHistory", parameters);
            return new DealHistoryResultVM
            {
                Rows = rows,
                RowCount = rows.Count
            };
        }
    }
}
