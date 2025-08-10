namespace OTS.DOMAIN.MobileAccoutingDTO
{
    public class TrialBalanceRequest
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public bool IncludeOpBal { get; set; }
        public int? GroupId { get; set; } = 0; // Optional: default = all
    }
}
