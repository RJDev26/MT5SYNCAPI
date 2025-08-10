namespace OTS.DOMAIN.MobileAccoutingDTO
{
    public class AccountGroupActionRequest
    {
        public int GroupId { get; set; }
        public string AccountIds { get; set; } = string.Empty; // e.g., "1,2,3"
        public string Action { get; set; } = "ADD"; // default to "ADD"
    }

}
