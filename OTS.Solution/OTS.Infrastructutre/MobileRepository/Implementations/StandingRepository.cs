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
    public class StandingRepository : IStandingRepository
    {
        private readonly DbManager _db;

        public StandingRepository(DbManager db)
        {
            _db = db;
        }

        public async Task<StandingResultVM> GetStandingAsync(DateOnly onDate, long? login, string? symbol, string? option, CancellationToken ct)
        {
            var parameters = new List<DbParameter>
            {
                new DbParameter("OnDate", ParameterDirection.Input, onDate.ToDateTime(TimeOnly.MinValue)),
                new DbParameter("Login", ParameterDirection.Input, login),
                new DbParameter("Symbol", ParameterDirection.Input, symbol),
                new DbParameter("Option", ParameterDirection.Input, option)
            };

            var rows = await _db.ExecuteListAsync<StandingVM>("usp_GetStandingByDate", parameters);
            return new StandingResultVM
            {
                Rows = rows,
                RowCount = rows.Count
            };
        }
    }
}
