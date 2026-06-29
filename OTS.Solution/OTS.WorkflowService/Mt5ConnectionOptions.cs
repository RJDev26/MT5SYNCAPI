namespace OTS.WorkflowService;

public sealed class Mt5ConnectionOptions
{
    public const string SectionName = "Mt5";

    public string Server { get; set; } = string.Empty;

    public int Port { get; set; } = 443;

    public long Login { get; set; }

    public string ManagerName { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public int PollIntervalSeconds { get; set; } = 30;

    public int ConnectTimeoutSeconds { get; set; } = 10;

    public string OutputDirectory { get; set; } = "mt5-data";
}
