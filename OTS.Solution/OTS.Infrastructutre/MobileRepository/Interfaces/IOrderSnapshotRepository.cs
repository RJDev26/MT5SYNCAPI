using System;
using System.Threading;
using System.Threading.Tasks;
using OTS.DOMAIN.MobileAccountingVM;

namespace MobileAccounting.Repositories.Interfaces
{
    public interface IOrderSnapshotRepository
    {
        Task<OrderSnapshotResultVM> GetOrdersSnapshotAsync(string? symbol, long? orderId, int? top, CancellationToken ct);
    }
}
