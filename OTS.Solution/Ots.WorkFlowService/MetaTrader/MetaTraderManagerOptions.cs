namespace Ots.WorkFlowService.MetaTrader;

public sealed class MetaTraderManagerOptions
{
    public const string SectionName = "MetaTraderManager";

    public string Server { get; set; } = string.Empty;

    public ulong ManagerLogin { get; set; }

    public string Password { get; set; } = string.Empty;

    public string ManagerName { get; set; } = string.Empty;

    public ulong TradingLogin { get; set; }

    public DateTime? HistoryFromUtc { get; set; }

    public TimeSpan PollInterval { get; set; } = TimeSpan.FromMinutes(1);

    public bool Enabled { get; set; } = true;
}
