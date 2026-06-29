namespace OTS.WorkflowService;

public interface IMt5ManagerClient
{
    Task<Mt5SyncSnapshot> GetOrdersAndDealsAsync(CancellationToken cancellationToken);
}
