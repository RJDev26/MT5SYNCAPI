using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OTS.DOMAIN.MobileAccountingVM;

namespace OTS.Service.Interfaces
{
    public interface IOrderSnapshotService
    {
        Task<List<OrderSnapshotVM>> GetOrdersSnapshotAsync(DateOnly onDate, CancellationToken ct);
    }
}
