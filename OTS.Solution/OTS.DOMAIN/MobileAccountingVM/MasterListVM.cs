using System;

namespace OTS.DOMAIN.MobileAccountingVM
{
    public class MasterListVM
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
    }
}
