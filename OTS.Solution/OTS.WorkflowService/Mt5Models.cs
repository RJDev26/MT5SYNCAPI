using System.Text.Json.Serialization;

namespace OTS.WorkflowService;

public sealed record Mt5OrderSnapshot(
    [property: JsonPropertyName("order")] long Order,
    [property: JsonPropertyName("login")] long Login,
    [property: JsonPropertyName("symbol")] string Symbol,
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("volume")] double Volume,
    [property: JsonPropertyName("price")] double Price,
    [property: JsonPropertyName("time")] DateTimeOffset Time);

public sealed record Mt5DealSnapshot(
    [property: JsonPropertyName("deal")] long Deal,
    [property: JsonPropertyName("login")] long Login,
    [property: JsonPropertyName("symbol")] string Symbol,
    [property: JsonPropertyName("action")] string Action,
    [property: JsonPropertyName("volume")] double Volume,
    [property: JsonPropertyName("price")] double Price,
    [property: JsonPropertyName("profit")] double Profit,
    [property: JsonPropertyName("time")] DateTimeOffset Time);

public sealed record Mt5SyncSnapshot(
    DateTimeOffset SyncedAt,
    string Server,
    long ManagerLogin,
    IReadOnlyCollection<Mt5OrderSnapshot> Orders,
    IReadOnlyCollection<Mt5DealSnapshot> Deals);
