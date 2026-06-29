namespace OTS.WorkflowService.Models
{
    /// <summary>
    /// Represents a single MT5 order (a trade instruction).
    /// </summary>
    public class Mt5Order
    {
        public ulong Order { get; set; }
        public ulong Login { get; set; }
        public string Symbol { get; set; } = string.Empty;
        public int Type { get; set; }
        public double VolumeInitial { get; set; }
        public double VolumeCurrent { get; set; }
        public double PriceOpen { get; set; }
        public double PriceCurrent { get; set; }
        public DateTime TimeSetup { get; set; }
        public string Comment { get; set; } = string.Empty;
    }

    /// <summary>
    /// Represents a single MT5 deal (an execution event resulting from an order).
    /// </summary>
    public class Mt5Deal
    {
        public ulong Deal { get; set; }
        public ulong Order { get; set; }
        public ulong Login { get; set; }
        public string Symbol { get; set; } = string.Empty;
        public int Entry { get; set; }
        public double Volume { get; set; }
        public double Price { get; set; }
        public double Profit { get; set; }
        public DateTime Time { get; set; }
    }

    /// <summary>
    /// Result of one synchronization pass.
    /// </summary>
    public class Mt5SyncResult
    {
        public IReadOnlyList<Mt5Order> Orders { get; set; } = Array.Empty<Mt5Order>();
        public IReadOnlyList<Mt5Deal> Deals { get; set; } = Array.Empty<Mt5Deal>();
        public DateTime FromUtc { get; set; }
        public DateTime ToUtc { get; set; }
    }
}
