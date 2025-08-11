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
    public class OrderSnapshotRepository : IOrderSnapshotRepository
    {
        private readonly DbManager _db;

        public OrderSnapshotRepository(DbManager db)
        {
            _db = db;
        }

        public Task<List<OrderSnapshotVM>> GetOrdersSnapshotAsync(DateOnly onDate, CancellationToken ct)
        {
            var parameters = new List<DbParameter>
            {
                new DbParameter("OnDate", ParameterDirection.Input, onDate.ToDateTime(TimeOnly.MinValue))
            };

            return _db.ExecuteListAsync<OrderSnapshotVM>("usp_GetOrdersSnapshot", parameters);
        }
    }
}
