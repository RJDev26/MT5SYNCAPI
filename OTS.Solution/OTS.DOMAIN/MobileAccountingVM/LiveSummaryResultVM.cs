using System.Collections.Generic;

namespace OTS.DOMAIN.MobileAccountingVM
{
    public class LiveSummaryResultVM
    {
        public IEnumerable<LiveSummaryVM> Rows { get; set; } = new List<LiveSummaryVM>();
        public int RowCount { get; set; }
    }
}
