using System;
using System.Collections.Generic;

namespace OTS.DOMAIN.MobileAccountingVM
{
    public class JobbingDealResultVM
    {
        public List<JobbingDealVM> Rows { get; set; } = new();
        public DateTime? MaxTime { get; set; }
        public long? RowsCount { get; set; }
    }
}
