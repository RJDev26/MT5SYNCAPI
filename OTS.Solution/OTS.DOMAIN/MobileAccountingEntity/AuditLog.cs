using System.ComponentModel.DataAnnotations;

namespace MobileAccounting.Entities
{
    public class AuditLog
    {
        [Key]
        public int LogId { get; set; }
        public int UserId { get; set; }
        public string ActionType { get; set; }
        public string TargetTable { get; set; }
        public int RecordId { get; set; }
        public DateTime Timestamp { get; set; }
        public string IpDeviceInfo { get; set; }
    }
}