using System;

namespace OTS.DOMAIN.MobileAccountingVM
{
    public class CrossTradePairDetailVM
    {
        public string? Symbol { get; set; }
        public string? LastIP { get; set; }
        public long? Login1 { get; set; }
        public long? Login2 { get; set; }
        public string? RowSide { get; set; }
        public long? Login { get; set; }
        public DateTime? Time { get; set; }
        public long? Deal { get; set; }
        public string? ConType { get; set; }
        public decimal? Qty { get; set; }
        public decimal? Price { get; set; }
        public decimal? Volume { get; set; }
        public decimal? Volumeext { get; set; }
        public decimal? Profit { get; set; }
        public decimal? Commission { get; set; }
        public string? Comment { get; set; }
    }
}
