using System;

namespace OTS.DOMAIN.MobileAccountingVM
{
    public class DealHistoryVM
    {
        public long Login { get; set; }
        public DateTime Time { get; set; }
        public long Deal { get; set; }
        public string? Symbol { get; set; }
        public string? Contype { get; set; }
        public int Entry { get; set; }
        public decimal Qty { get; set; }
        public decimal Price { get; set; }
        public decimal Volume { get; set; }
        public decimal VolumeExt { get; set; }
        public decimal Profit { get; set; }
        public decimal Commission { get; set; }
        public string? Comment { get; set; }
    }
}
