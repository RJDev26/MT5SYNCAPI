using System.ComponentModel.DataAnnotations;

namespace OTS.DOMAIN.MobileAccountingVM
{
    public class UserManagerMappingRequestVM
    {
        public int UserId { get; set; }

        [Required]
        public string ManagerIds { get; set; } = string.Empty;

        public string Action { get; set; } = "INSERT";
    }
}
