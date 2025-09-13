using System.Collections.Generic;

namespace OTS.DOMAIN.MobileAccountingVM
{
    public class DealHistoryResultVM
    {
        public IEnumerable<DealHistoryVM> Rows { get; set; } = new List<DealHistoryVM>();
        public int RowCount { get; set; }
    }
}
