namespace OTS.DOMAIN.MobileAccountingVM
{
    public class TrialBalanceRowDto
    {
        public string Dr_ShortCode { get; set; }
        public string Dr_Name { get; set; }
        public decimal? Debit { get; set; }
        public string Cr_ShortCode { get; set; }
        public string Cr_Name { get; set; }
        public decimal? Credit { get; set; }
    }

    public class TrialBalanceSummaryDto
    {
        public decimal TotalDebit { get; set; }
        public decimal TotalCredit { get; set; }
        public decimal Difference { get; set; }
    }

    public class TrialBalanceResultDto
    {
        public List<TrialBalanceRowDto> Rows { get; set; }
        public TrialBalanceSummaryDto Summary { get; set; }
    }

}
