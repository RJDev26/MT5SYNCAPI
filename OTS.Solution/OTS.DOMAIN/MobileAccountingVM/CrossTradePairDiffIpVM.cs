using System;

namespace OTS.DOMAIN.MobileAccountingVM
{
    public class CrossTradePairDiffIpVM
    {
        public string? Symbol { get; set; }
        public long? Login1 { get; set; }
        public string? IP1 { get; set; }
        public long? Login2 { get; set; }
        public string? IP2 { get; set; }
        public long? BuyDeal { get; set; }
        public long? SellDeal { get; set; }
        public decimal? Qty { get; set; }
        public DateTime? BuyTime { get; set; }
        public DateTime? SellTime { get; set; }
        public int? DiffSec { get; set; }
        public decimal? BuyProfit { get; set; }
        public decimal? SellProfit { get; set; }
        public decimal? TotalProfit { get; set; }
    }
}
