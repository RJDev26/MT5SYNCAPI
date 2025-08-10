using System.ComponentModel.DataAnnotations;

namespace MobileAccounting.Entities
{
    public class VoucherMaster
    {
        [Key]
        public int VoucherId { get; set; }
        public DateTime VoucherDate { get; set; }
        public string VoucherType { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}