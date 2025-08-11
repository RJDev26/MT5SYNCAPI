using System;

namespace OTS.DOMAIN.MobileAccountingVM
{
    public class JobbingDealVM
    {
        public long Login { get; set; }
        public DateTime? BuyTime { get; set; }
        public DateTime? SellTime { get; set; }
        public long BuyDeal { get; set; }
        public long SellDeal { get; set; }
        public double DiffSec { get; set; }
        public string Symbol { get; set; }
        public string BuySymbol { get; set; }
        public string SellSymbol { get; set; }
        public string? International { get; set; }
        public double BQty { get; set; }
        public double SQty { get; set; }
        public double BuyPrice { get; set; }
        public double SellPrice { get; set; }
        public double PriceDiff { get; set; }
        public double MTM { get; set; }
        public double Comm { get; set; }
        public double CommR { get; set; }
        public double MTMR { get; set; }
        public DateTime Date { get; set; }
        public string DateString { get; set; }
        public string BuyTimeString { get; set; }
        public string SellTimeString { get; set; }
    }
}
