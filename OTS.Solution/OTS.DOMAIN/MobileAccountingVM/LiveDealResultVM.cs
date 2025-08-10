using System;
using System.Collections.Generic;

namespace OTS.DOMAIN.MobileAccountingVM
{
    public class LiveDealResultVM
    {
        public List<LiveDealVM> Rows { get; set; } = new();
        public DateTime? MaxTime { get; set; }
        public int? RowCount { get; set; }
    }
}
