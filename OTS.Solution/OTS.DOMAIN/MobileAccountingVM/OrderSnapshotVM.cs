using System;

namespace OTS.DOMAIN.MobileAccountingVM
{
    public class OrderSnapshotVM
    {
        public long Login { get; set; }
        public DateTime Time { get; set; }
        public long Order { get; set; }
        public string Symbol { get; set; }
        public double Qty { get; set; }
        public double Price { get; set; }
        public double Volume { get; set; }
        public int OrderType { get; set; }
        public string OrderTypeName { get; set; }
        public string LastIP { get; set; }
    }
}
