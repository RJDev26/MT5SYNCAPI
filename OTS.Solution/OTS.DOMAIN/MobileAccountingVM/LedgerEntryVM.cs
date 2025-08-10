namespace OTS.DOMAIN.MobileAccountingVM
{
    public class LedgerEntryVM
    {
        public DateTime EntryDate { get; set; }
        public string VOU_TYPE { get; set; }
        public string DR_CR { get; set; }
        public int ACCID { get; set; }
        public string AccountName { get; set; }
        public string AccountCode { get; set; }
        public string Narration { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public decimal Balance { get; set; }
    }
}
