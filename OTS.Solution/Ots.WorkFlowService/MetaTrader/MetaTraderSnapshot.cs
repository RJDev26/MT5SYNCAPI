namespace Ots.WorkFlowService.MetaTrader;

public sealed record MetaTraderSnapshot(
    DateTime From,
    DateTime To,
    IReadOnlyList<MetaTraderDeal> Deals,
    IReadOnlyList<MetaTraderOrder> Orders);
