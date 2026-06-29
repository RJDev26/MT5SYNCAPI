namespace OTS.WorkflowService.Options
{
    /// <summary>
    /// Strongly-typed configuration for the MT5 synchronization worker.
    /// Bound from the "Mt5Sync" section of appsettings.json.
    /// </summary>
    public class Mt5SyncOptions
    {
        public const string SectionName = "Mt5Sync";

        /// <summary>MT5 server address (IP or named access server).</summary>
        public string Server { get; set; } = string.Empty;

        /// <summary>Manager / account login id.</summary>
        public ulong Login { get; set; }

        /// <summary>
        /// Account password. Do NOT store real secrets in appsettings.json.
        /// Use user-secrets, environment variables, or a secret manager.
        /// </summary>
        public string Password { get; set; } = string.Empty;

        /// <summary>How often the worker polls MT5 for new deals/orders.</summary>
        public int PollIntervalSeconds { get; set; } = 30;

        /// <summary>
        /// Look-back window (in minutes) used on the first sync pass when no
        /// previous high-water mark is available.
        /// </summary>
        public int InitialLookbackMinutes { get; set; } = 60;
    }
}
