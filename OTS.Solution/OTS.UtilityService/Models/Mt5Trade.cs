namespace OTS.UtilityService.Models;

public sealed record Mt5Trade(
    ulong DealId,
    ulong Login,
    string Symbol,
    decimal Volume,
    decimal Price,
    DateTimeOffset Time,
    string Side);
