using System.ComponentModel.DataAnnotations;

namespace MobileAccounting.Entities
{
    public class VoucherMasterDeleted
    {
        [Key]
        public int DeletedId { get; set; }
        public int VoucherId { get; set; }
        public DateTime VoucherDate { get; set; }
        public string VoucherType { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public int DeletedBy { get; set; }
        public DateTime DeletedAt { get; set; }
    }
}