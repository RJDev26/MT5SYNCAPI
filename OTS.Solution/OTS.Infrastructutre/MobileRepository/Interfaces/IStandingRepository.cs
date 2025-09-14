using System;
using System.Threading;
using System.Threading.Tasks;
using OTS.DOMAIN.MobileAccountingVM;

namespace MobileAccounting.Repositories.Interfaces
{
    public interface IStandingRepository
    {
        Task<StandingResultVM> GetStandingAsync(DateOnly onDate, long? login, string? symbol, string? option, CancellationToken ct);
    }
}
