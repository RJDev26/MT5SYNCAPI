namespace OTS.DOMAIN.MobileAccountingVM
{
    public class VoucherEntryRequest
    {
        public int? VoucherId { get; set; }
        public DateTime VoucherDate { get; set; }
        public string VoucherType { get; set; }
        public int CreatedBy { get; set; }
        public int DrAccountId { get; set; }
        public int CrAccountId { get; set; }
        public decimal Amount { get; set; }
        public string Narration { get; set; }
    }


}
