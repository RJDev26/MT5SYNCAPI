using System.ComponentModel.DataAnnotations;

namespace OTS.UtilityService.Options;

public sealed class Mt5ManagerOptions
{
    public const string SectionName = "Mt5Manager";

    [Required]
    public string Server { get; init; } = string.Empty;

    [Range(1, 65535)]
    public int Port { get; init; } = 443;

    [Required]
    public string ManagerName { get; init; } = string.Empty;

    [Range(1, long.MaxValue)]
    public long Login { get; init; }

    [Required]
    public string Password { get; init; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int PollIntervalSeconds { get; init; } = 30;

    [Range(1, int.MaxValue)]
    public int ConnectionTimeoutSeconds { get; init; } = 15;
}
