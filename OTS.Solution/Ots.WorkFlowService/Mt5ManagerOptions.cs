namespace Ots.WorkFlowService;

public sealed class Mt5ManagerOptions
{
    public const string SectionName = "Mt5Manager";

    public ulong Login { get; set; } = 49600;

    public string Password { get; set; } = string.Empty;

    public string Server { get; set; } = "85.195.95.30";

    public int Port { get; set; } = 443;

    public string ManagerName { get; set; } = "A PATEL 5%";

    public int DealHistoryLookbackDays { get; set; } = 1;
}
