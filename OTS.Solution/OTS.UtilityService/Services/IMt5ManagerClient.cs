using OTS.UtilityService.Models;

namespace OTS.UtilityService.Services;

public interface IMt5ManagerClient
{
    Task<IReadOnlyCollection<Mt5Trade>> GetTradesAsync(DateTimeOffset from, CancellationToken cancellationToken);
}
