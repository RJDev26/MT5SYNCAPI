using System;

namespace OTS.DOMAIN.MobileAccountingVM
{
    public class LiveDealVM
    {
        public long Login { get; set; }
        public DateTime Time { get; set; }
        public long Deal { get; set; }
        public string Symbol { get; set; }
        public string Contype { get; set; }
        public int Entry { get; set; }
        public double Qty { get; set; }
        public double Price { get; set; }
        public double Volume { get; set; }
        public double Volumeext { get; set; }
        public double Profit { get; set; }
        public double Commission { get; set; }
        public string LastIP { get; set; }
        public string? Comment { get; set; }
    }
}
