using System.Text.Json.Serialization;

namespace OTS.DOMAIN.MobileAccountingVM
{
    public class ManagerSummaryVM
    {
        [JsonPropertyName("managerId")]
        public int ManagerId { get; set; }

        [JsonPropertyName("managerName")]
        public string ManagerName { get; set; } = string.Empty;
    }
}
