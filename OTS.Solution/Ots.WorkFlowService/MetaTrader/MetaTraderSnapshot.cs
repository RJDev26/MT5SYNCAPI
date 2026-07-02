namespace Ots.WorkFlowService.MetaTrader;

public sealed record MetaTraderSnapshot(
    DateTime FromUtc,
    DateTime ToUtc,
    ulong TradingLogin,
    object? Deals,
    object? Orders,
    object? PendingOrders);
