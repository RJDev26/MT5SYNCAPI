namespace OTS.DOMAIN.MobileAccountingVM
{
    public class VoucherSummaryVM
    {
        public int VoucherId { get; set; }
        public DateTime Date { get; set; }
        public string VouType { get; set; }
        public string DrCr { get; set; }
        public decimal Amount { get; set; }
        public string DrAccount { get; set; }
        public string CrAccount { get; set; }
        public bool Confirmed { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public string Narration { get; set; }
    }

}
