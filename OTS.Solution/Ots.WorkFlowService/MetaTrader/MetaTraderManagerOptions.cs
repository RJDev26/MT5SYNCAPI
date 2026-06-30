namespace Ots.WorkFlowService.MetaTrader;

public sealed class MetaTraderManagerOptions
{
    public const string SectionName = "MetaTraderManager";

    public string Server { get; set; } = string.Empty;

    public ulong Login { get; set; }

    public string Password { get; set; } = string.Empty;

    public string? ManagerName { get; set; }

    public uint PumpingMode { get; set; }

    public int TimeoutMilliseconds { get; set; } = 30_000;

    public int PollIntervalSeconds { get; set; } = 5;

    public int LookbackMinutes { get; set; } = 5;

    public int MaxRowsPerPoll { get; set; } = 500;

    public string[] AssemblyNames { get; set; } =
    [
        "MetaQuotes.MT5ManagerAPI",
        "MT5ManagerAPI",
        "MetaTrader5.ManagerAPI"
    ];
}
