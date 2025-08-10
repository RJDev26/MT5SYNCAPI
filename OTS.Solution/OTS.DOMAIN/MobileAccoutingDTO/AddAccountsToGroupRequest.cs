namespace OTS.DOMAIN.MobileAccoutingDTO
{
    public class AddAccountsToGroupRequest
    {
        public int GroupId { get; set; }
        public List<int> AccountIds { get; set; } = new();
    }

}
