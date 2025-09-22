using System;

namespace OTS.DOMAIN.MobileAccountingVM
{
    public class CrossTradePairVM
    {
        public string? Symbol { get; set; }
        public string? LastIP { get; set; }
        public long? Login1 { get; set; }
        public long? Login2 { get; set; }
        public DateTime? FirstTradeTime { get; set; }
        public DateTime? LastTradeTime { get; set; }
        public int? Deals { get; set; }
        public int? BDeals { get; set; }
        public int? SDeals { get; set; }
    }
}
