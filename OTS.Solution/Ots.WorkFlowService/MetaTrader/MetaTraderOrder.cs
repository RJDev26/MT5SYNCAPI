namespace Ots.WorkFlowService.MetaTrader;

public sealed record MetaTraderOrder(
    ulong Order,
    ulong Login,
    string Symbol,
    string Type,
    string State,
    double VolumeInitial,
    double VolumeCurrent,
    double PriceOpen,
    DateTime TimeSetup);
