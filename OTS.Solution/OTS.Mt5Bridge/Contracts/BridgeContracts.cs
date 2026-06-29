using System;
using System.Collections.Generic;

namespace OTS.Mt5Bridge.Contracts
{
    // Wire DTOs returned by the bridge over HTTP. Must stay in sync with the
    // matching client-side types in OTS.WorkflowService.Bridge.

    public class OrderDto
    {
        public ulong Order { get; set; }
        public ulong Login { get; set; }
        public string Symbol { get; set; } = string.Empty;
        public int Type { get; set; }
        public double VolumeInitial { get; set; }
        public double VolumeCurrent { get; set; }
        public double PriceOpen { get; set; }
        public double PriceCurrent { get; set; }
        public DateTime TimeSetupUtc { get; set; }
        public string Comment { get; set; } = string.Empty;
    }

    public class DealDto
    {
        public ulong Deal { get; set; }
        public ulong Order { get; set; }
        public ulong Login { get; set; }
        public string Symbol { get; set; } = string.Empty;
        public int Entry { get; set; }
        public double Volume { get; set; }
        public double Price { get; set; }
        public double Profit { get; set; }
        public DateTime TimeUtc { get; set; }
    }

    public class SyncResponse<T>
    {
        public bool Connected { get; set; }
        public string? Error { get; set; }
        public List<T> Items { get; set; } = new List<T>();
    }
}
