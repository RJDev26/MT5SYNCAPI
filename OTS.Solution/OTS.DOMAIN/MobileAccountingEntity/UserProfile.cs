using System.ComponentModel.DataAnnotations;

namespace MobileAccounting.Entities
{
    public class UserProfile
    {
        [Key]
        public int ProfileId { get; set; }
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Gender { get; set; }
        public DateTime? Dob { get; set; }
        public string Address { get; set; }
        public string ProfileImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}