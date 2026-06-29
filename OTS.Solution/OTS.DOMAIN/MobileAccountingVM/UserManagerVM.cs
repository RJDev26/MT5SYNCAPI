using System;

namespace OTS.DOMAIN.MobileAccountingVM
{
    public class UserManagerVM
    {
        public int ManagerId { get; set; }

        public int UserId { get; set; }

        public string ManagerName { get; set; } = string.Empty;

        public string UserName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;
    }
}
