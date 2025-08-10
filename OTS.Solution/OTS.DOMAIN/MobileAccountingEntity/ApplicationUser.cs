using Microsoft.AspNetCore.Identity;

namespace MobileAccounting.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public int UserId { get; set; }
        public string? Role { get; set; }
    }
}
