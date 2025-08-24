namespace OTS.DOMAIN.MobileAccountingVM
{
    public class LoginResponseVM
    {
        public bool IsAuthSuccessful { get; set; }
        public string? ErrorMessage { get; set; }
        public string Token { get; set; } = string.Empty;
        public string? UserName { get; set; }
        public int UserId { get; set; }
        public string Id { get; set; } = string.Empty;
        public string? Role { get; set; }
    }
}
