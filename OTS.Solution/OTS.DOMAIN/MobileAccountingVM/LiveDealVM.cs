using System;

namespace OTS.DOMAIN.MobileAccountingVM
{
    public class LiveDealVM
    {
        public long Ticket { get; set; }
        public long Order { get; set; }
        public string Symbol { get; set; }
        public string Action { get; set; }
        public double Volume { get; set; }
        public double Price { get; set; }
        public DateTime Time { get; set; }
    }
}
