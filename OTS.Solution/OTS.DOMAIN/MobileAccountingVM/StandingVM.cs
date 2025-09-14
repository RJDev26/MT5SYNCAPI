using System;

namespace OTS.DOMAIN.MobileAccountingVM
{
    public class StandingVM
    {
        public long Login { get; set; }
        public string? Symbol { get; set; }
        public DateTime ExpiryDate { get; set; }
        public decimal BuyQty { get; set; }
        public decimal SellQty { get; set; }
        public decimal NetQty { get; set; }
        public decimal BrokerShare { get; set; }
        public decimal ManagerShare { get; set; }
    }
}
