namespace Ots.WorkFlowService.MetaTrader;

public sealed record MetaTraderDeal(
    ulong Deal,
    ulong Order,
    ulong Login,
    string Symbol,
    string Action,
    double Volume,
    double Price,
    double Profit,
    DateTime Time);
