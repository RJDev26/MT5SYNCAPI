using Microsoft.Extensions.Configuration;

namespace OTS.WorkflowService.Options
{
    /// <summary>
    /// Strongly-typed configuration for the MT5 synchronization worker.
    /// Bound from the "Mt5Sync" section of appsettings.json.
    /// </summary>
    public class Mt5SyncOptions
    {
        public const string SectionName = "Mt5Sync";

        /// <summary>MT5 manager login ID from the account_id configuration key.</summary>
        [ConfigurationKeyName("account_id")]
        public string AccountId { get; set; } = "49600";

        /// <summary>MT5 manager password from the password configuration key.</summary>
        [ConfigurationKeyName("password")]
        public string Password { get; set; } = "RTP@12345";

        /// <summary>MT5 server address from the server configuration key.</summary>
        [ConfigurationKeyName("server")]
        public string Server { get; set; } = "85.195.95.30";

        /// <summary>Parsed manager login ID used by MT5 bridge/native calls.</summary>
        public ulong Login => ulong.TryParse(AccountId, out var login) ? login : 0;

        /// <summary>
        /// Base URL of the net48 OTS.Mt5Bridge process that owns the native
        /// MT5 Manager API connection.
        /// </summary>
        public string BridgeBaseUrl { get; set; } = "http://127.0.0.1:5099";

        /// <summary>
        /// Comma-separated MT5 logins to sync. Leave "0" for every login the
        /// manager account can see.
        /// </summary>
        public string Logins { get; set; } = "0";

        /// <summary>How often the worker polls MT5 for new deals/orders.</summary>
        public int PollIntervalSeconds { get; set; } = 30;

        /// <summary>
        /// Look-back window (in minutes) used on the first sync pass when no
        /// previous high-water mark is available.
        /// </summary>
        public int InitialLookbackMinutes { get; set; } = 60;
    }
}
