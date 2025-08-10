namespace OTS.DOMAIN.MobileAccoutingDTO
{
    public class AccountGroupWithAccountsDto
    {
        public int GroupId { get; set; }
        public string GroupCode { get; set; }
        public string GroupName { get; set; }

        public int AccountId { get; set; }
        public string AccountCode { get; set; }
        public string AccountName { get; set; }

        public decimal? OpeningBalance { get; set; }
        public string DrCr { get; set; }
    }

}
