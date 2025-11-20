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

        public async Task<OrderSnapshotResultVM> GetOrdersSnapshotAsync(string? symbol, long? orderId, int? top, int userId, CancellationToken ct)
        {
            var parameters = new List<DbParameter>
            {
                new DbParameter("Symbol", ParameterDirection.Input, symbol ?? (object)DBNull.Value),
                new DbParameter("OrderId", ParameterDirection.Input, orderId ?? (object)DBNull.Value),
                new DbParameter("Top", ParameterDirection.Input, top ?? (object)DBNull.Value),
                new DbParameter("UserId", ParameterDirection.Input, userId)
            };

            var (rows, meta) = await _db.ExecuteMultipleAsync<OrderSnapshotVM, OrderSnapshotMetaVM>(
                "usp_GetOrdersSnapshot",
                parameters);

            return new OrderSnapshotResultVM
            {
                Rows = rows,
                MaxTime = meta?.MaxTime,
                TotalRows = meta?.TotalRows
            };
        }
    }
}
