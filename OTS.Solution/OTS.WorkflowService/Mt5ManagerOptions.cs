namespace OTS.WorkflowService;

public sealed class Mt5ManagerOptions
{
    public string Server { get; set; } = string.Empty;
    public ulong Login { get; set; }
    public string Password { get; set; } = string.Empty;
    public int PumpingMode { get; set; } = 0;
    public uint TimeoutMilliseconds { get; set; } = 60_000;
    public int PollIntervalSeconds { get; set; } = 30;
    public uint PreviewCount { get; set; } = 10;
}
