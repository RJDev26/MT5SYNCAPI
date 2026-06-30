namespace Ots.WorkFlowService.MetaTrader;

public interface IMetaTraderManagerClient
{
    Task<bool> CheckLoginAsync(CancellationToken cancellationToken = default);

    Task<MetaTraderSnapshot> GetDealsAndOrdersAsync(
        DateTime fromUtc,
        DateTime toUtc,
        ulong tradingLogin,
        CancellationToken cancellationToken = default);
}
