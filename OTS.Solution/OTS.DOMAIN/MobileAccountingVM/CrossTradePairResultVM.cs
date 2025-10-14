using System.Collections.Generic;

namespace OTS.DOMAIN.MobileAccountingVM
{
    public class CrossTradePairResultVM
    {
        public List<CrossTradePairVM> Pairs { get; set; } = new();
        public List<CrossTradePairDetailVM> Details { get; set; } = new();
    }
}
