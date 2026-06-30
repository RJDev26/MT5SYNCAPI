namespace OTS.WorkflowService;

public sealed class Mt5ManagerOptions
{
    public string Server { get; set; } = string.Empty;
    public ulong Login { get; set; }
    public string Password { get; set; } = string.Empty;
    public string? NativeLibraryPath { get; set; }
    public int PumpingMode { get; set; } = 0;
    public uint TimeoutMilliseconds { get; set; } = 60_000;
    public int PollIntervalSeconds { get; set; } = 30;
    public uint PreviewCount { get; set; } = 10;
    public uint MaxRows { get; set; } = 100;
    public string OrderGroupMask { get; set; } = "*";
    public ulong? DealHistoryLogin { get; set; }
    public int DealHistoryDays { get; set; } = 1;
}
