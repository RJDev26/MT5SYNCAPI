using System.ComponentModel.DataAnnotations;

namespace MobileAccounting.Entities
{
    public class AccountGroupMaster
    {
        [Key]
        public int GroupId { get; set; }
        public string GroupCode { get; set; }
        public string GroupName { get; set; }
    }
}