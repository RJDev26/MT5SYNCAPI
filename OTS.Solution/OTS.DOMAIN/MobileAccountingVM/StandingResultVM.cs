using System.Collections.Generic;

namespace OTS.DOMAIN.MobileAccountingVM
{
    public class StandingResultVM
    {
        public IEnumerable<StandingVM> Rows { get; set; } = new List<StandingVM>();
        public int RowCount { get; set; }
    }
}
