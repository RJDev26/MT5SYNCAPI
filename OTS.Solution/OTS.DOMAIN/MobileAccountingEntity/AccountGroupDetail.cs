using System.ComponentModel.DataAnnotations;

namespace MobileAccounting.Entities
{
    public class AccountGroupDetail
    {
        [Key]
        public int DetailId { get; set; }
        public int GroupId { get; set; }
        public int AccountId { get; set; }
    }
}