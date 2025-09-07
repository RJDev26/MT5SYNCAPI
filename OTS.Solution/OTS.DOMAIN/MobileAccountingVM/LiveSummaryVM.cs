using System;

namespace OTS.DOMAIN.MobileAccountingVM
{
    public class LiveSummaryVM
    {
        public long Login { get; set; }
        public string? Symbol { get; set; }
        public decimal OpenQty { get; set; }
        public decimal OpenRate { get; set; }
        public decimal OpenAmt { get; set; }
        public decimal BuyQty { get; set; }
        public decimal BuyAmt { get; set; }
        public decimal SellQty { get; set; }
        public decimal SellAmt { get; set; }
        public decimal Commission { get; set; }
        public decimal CloseQty { get; set; }
        public decimal CloseRate { get; set; }
        public decimal CloseAmt { get; set; }
        public decimal GrossMTM { get; set; }
        public decimal NetAmt { get; set; }
    }
}
