using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OTS.DOMAIN.MobileAccountingVM;

namespace MobileAccounting.Repositories.Interfaces
{
    public interface IOrderSnapshotRepository
    {
        Task<List<OrderSnapshotVM>> GetOrdersSnapshotAsync(DateOnly onDate, CancellationToken ct);
    }
}
