using System.ComponentModel.DataAnnotations;

namespace MobileAccounting.Entities
{
    public class VoucherDetailsDeleted
    {
        [Key]
        public int DeletedDetailId { get; set; }
        public int VoucherId { get; set; }
        public int AccountId { get; set; }
        public string DrCr { get; set; }
        public decimal Amount { get; set; }
        public string Narration { get; set; }
        public bool EntryConfirm { get; set; }
        public int DeletedBy { get; set; }
        public DateTime DeletedAt { get; set; }
    }
}