using System;
using System.Threading;
using System.Threading.Tasks;
using MobileAccounting.Repositories.Interfaces;
using OTS.DOMAIN.MobileAccountingVM;
using OTS.Service.Interfaces;

namespace OTS.Service
{
    public class OrderSnapshotService : IOrderSnapshotService
    {
        private readonly IOrderSnapshotRepository _repository;

        public OrderSnapshotService(IOrderSnapshotRepository repository)
        {
            _repository = repository;
        }

        public Task<OrderSnapshotResultVM> GetOrdersSnapshotAsync(string? symbol, long? orderId, int? top, int userId, CancellationToken ct)
        {
            return _repository.GetOrdersSnapshotAsync(symbol, orderId, top, userId, ct);
        }
    }
}
