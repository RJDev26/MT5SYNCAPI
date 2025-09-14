using System;
using System.Threading;
using System.Threading.Tasks;
using MobileAccounting.Repositories.Interfaces;
using OTS.DOMAIN.MobileAccountingVM;
using OTS.Service.Interfaces;

namespace OTS.Service
{
    public class StandingService : IStandingService
    {
        private readonly IStandingRepository _repository;

        public StandingService(IStandingRepository repository)
        {
            _repository = repository;
        }

        public Task<StandingResultVM> GetStandingAsync(DateOnly onDate, long? login, string? symbol, CancellationToken ct)
        {
            return _repository.GetStandingAsync(onDate, login, symbol, ct);
        }
    }
}
