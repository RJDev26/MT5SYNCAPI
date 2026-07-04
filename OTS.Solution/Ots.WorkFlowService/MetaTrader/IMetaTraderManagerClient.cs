namespace Ots.WorkFlowService.MetaTrader;

public interface IMetaTraderManagerClient
{
    Task<MetaTraderSnapshot> GetLiveDealsAndOrdersAsync(DateTime from, DateTime to, CancellationToken cancellationToken);
}
