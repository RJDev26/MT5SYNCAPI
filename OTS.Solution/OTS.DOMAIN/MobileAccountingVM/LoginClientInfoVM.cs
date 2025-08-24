using System;

namespace OTS.DOMAIN.MobileAccountingVM
{
    public class LoginClientInfoVM
    {
        public long Login { get; set; }
        public string UserName { get; set; } = string.Empty;
        public bool HasClientRecord { get; set; }
        public int? ClientId { get; set; }
        public int? ManagerId { get; set; }
        public string ManagerName { get; set; } = string.Empty;
        public string ManagerCode { get; set; } = string.Empty;
        public int? BrokerId { get; set; }
        public string BrokerName { get; set; } = string.Empty;
        public string BrokerCode { get; set; } = string.Empty;
        public int? ExId { get; set; }
        public string ExCode { get; set; } = string.Empty;
        public string Exchange { get; set; } = string.Empty;
        public double? BrokShare { get; set; }
        public double? ManagerShare { get; set; }
        public string Currency { get; set; } = string.Empty;
        public double? Commission { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? CreatedDate { get; set; }
    }
}
