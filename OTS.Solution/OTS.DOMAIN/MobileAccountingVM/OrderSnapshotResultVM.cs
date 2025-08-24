using System;
using System.Collections.Generic;

namespace OTS.DOMAIN.MobileAccountingVM
{
    public class OrderSnapshotResultVM
    {
        public List<OrderSnapshotVM> Rows { get; set; } = new();
        public DateTime? MaxTime { get; set; }
        public long? TotalRows { get; set; }
    }
}
