namespace Ots.WorkFlowService.MetaTrader;

public interface IMetaTraderManagerClient
{
    Task<MetaTraderSnapshot> GetDealsAndOrdersAsync(
        DateTime fromUtc,
        DateTime toUtc,
        ulong tradingLogin,
        CancellationToken cancellationToken = default);
}
