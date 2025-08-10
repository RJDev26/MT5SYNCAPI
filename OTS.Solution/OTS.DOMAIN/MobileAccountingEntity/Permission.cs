using System.ComponentModel.DataAnnotations;

namespace MobileAccounting.Entities
{
    public class Permission
    {
        [Key]
        public int PermissionId { get; set; }
        public string PermissionName { get; set; }
        public string Description { get; set; }
    }
}