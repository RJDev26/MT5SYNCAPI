namespace OTS.DOMAIN.MobileAccountingVM
{
    public class VoucherDetailVM
    {
        public int VoucherId { get; set; }
        public DateTime Date { get; set; }
        public string VoucherType { get; set; }
        public int DrAccountId { get; set; }
        public int CrAccountId { get; set; }
        public string DrCr { get; set; }
        public decimal Amount { get; set; }
        public string Narration { get; set; }
        public bool Confirmed { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CreatedBy { get; set; }
    }

}
