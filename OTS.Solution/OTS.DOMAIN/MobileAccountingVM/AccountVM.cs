namespace OTS.DOMAIN.MobileAccountingVM
{
    public class AccountVM
    {
        public int AccountId { get; set; }
        public string AccountName { get; set; }
        public string ShortCode { get; set; }
        public string HeadName { get; set; }
        public int AccountHead { get; set; }
        public int AcountLevel { get; set; }
        public int AcGroup { get; set; }
        public decimal OpBal { get; set; }
        public string DRCR { get; set; }
        public decimal BaseRate { get; set; }
        public int CurrencyMaster { get; set; }
        public string CurrencyName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }

}
