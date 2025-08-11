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

        public Task<List<OrderSnapshotVM>> GetOrdersSnapshotAsync(string? symbol, long? orderId, int? top, CancellationToken ct)
        {
            var parameters = new List<DbParameter>
            {
                new DbParameter("Symbol", ParameterDirection.Input, symbol ?? (object)DBNull.Value),
                new DbParameter("OrderId", ParameterDirection.Input, orderId ?? (object)DBNull.Value),
                new DbParameter("Top", ParameterDirection.Input, top ?? (object)DBNull.Value)
            };

            return _db.ExecuteListAsync<OrderSnapshotVM>("usp_GetOrdersSnapshot", parameters);
        }
    }
}
