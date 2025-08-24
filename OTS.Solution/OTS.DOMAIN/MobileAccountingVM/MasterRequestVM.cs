using System.ComponentModel.DataAnnotations;

namespace OTS.DOMAIN.MobileAccountingVM
{
    public class MasterRequestVM
    {
        [Required]
        public string TableName { get; set; } = string.Empty;

        [Required]
        public int Id { get; set; }

        [Required]
        public string Code { get; set; } = string.Empty;

        [Required]
        public string Name { get; set; } = string.Empty;
    }
}
