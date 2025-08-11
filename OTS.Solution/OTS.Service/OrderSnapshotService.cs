using System;
using System.Collections.Generic;
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

        public Task<List<OrderSnapshotVM>> GetOrdersSnapshotAsync(DateOnly onDate, CancellationToken ct)
        {
            return _repository.GetOrdersSnapshotAsync(onDate, ct);
        }
    }
}
