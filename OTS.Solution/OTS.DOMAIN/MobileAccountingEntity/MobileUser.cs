using System.ComponentModel.DataAnnotations;

namespace MobileAccounting.Entities
{
    public class MobileUser
    {
        [Key]
        public int MobileUserId { get; set; }
        public int UserId { get; set; }
        public string MobileNo { get; set; }
        public string DeviceId { get; set; }
        public DateTime? LastLoggedIn { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}