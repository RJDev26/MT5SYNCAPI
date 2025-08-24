using System;

namespace OTS.DOMAIN.MobileAccountingVM
{
    public class StandingVM
    {
        public DateTime TradeDate { get; set; }
        public long Login { get; set; }
        public string? Symbol { get; set; }
        public decimal BuyQty { get; set; }
        public decimal SellQty { get; set; }
    }
}
