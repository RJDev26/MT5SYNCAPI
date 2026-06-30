using mtapi.mt5;

namespace Ots.WorkFlowService;

public sealed record Mt5ManagerSnapshot(
    IReadOnlyCollection<Order> OpenedOrders,
    IReadOnlyCollection<Order> DealHistoryOrders,
    IReadOnlyCollection<object> InternalDeals,
    IReadOnlyCollection<object> InternalOrders);
