using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MobileAccounting.Entities
{
    [Table("account_master")]
    public class AccountMaster
    {
        [Key]  
        
        public int AccountId { get; set; }
        public string ShortCode { get; set; }
        public string Name { get; set; }
        public string drcr { get; set; }
        public decimal OpeningBalance { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}