namespace OTS.DOMAIN.MobileAccountingVM
{
    public class LoginResponseVM
    {
        public string Token { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string? Role { get; set; }
        public string? Name { get; set; }
    }
}
