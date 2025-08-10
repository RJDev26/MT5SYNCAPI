namespace OTS.DOMAIN.MobileAccoutingDTO
{
    public class AccountGroupRequest
    {
        public int? GroupId { get; set; }  // 0 or null means new
        public string GroupCode { get; set; }
        public string GroupName { get; set; }
    }

}
