namespace OTS.DOMAIN.MobileAccountingVM
{
    public class ClientMasterRequestVM
    {
        public string Action { get; set; } = string.Empty;
        public int? Id { get; set; }
        public long? Login { get; set; }
        public int? ManagerId { get; set; }
        public int? BrokerId { get; set; }
        public int? ExId { get; set; }
        public decimal? BrokShare { get; set; }
        public decimal? ManagerShare { get; set; }
        public string? Currency { get; set; }
        public decimal? Commission { get; set; }
        public int? CreatedBy { get; set; }
    }
}
