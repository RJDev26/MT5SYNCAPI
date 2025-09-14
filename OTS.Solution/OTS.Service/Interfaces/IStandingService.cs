using System;
using System.Threading;
using System.Threading.Tasks;
using OTS.DOMAIN.MobileAccountingVM;

namespace OTS.Service.Interfaces
{
    public interface IStandingService
    {
        Task<StandingResultVM> GetStandingAsync(DateOnly onDate, long? login, string? symbol, CancellationToken ct);
    }
}
