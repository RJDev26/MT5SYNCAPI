using OTS.WorkflowService;
using OTS.WorkflowService.Options;
using OTS.WorkflowService.Services;

var builder = Host.CreateApplicationBuilder(args);

// Bind MT5 sync configuration from the "Mt5Sync" section.
builder.Services.Configure<Mt5SyncOptions>(
    builder.Configuration.GetSection(Mt5SyncOptions.SectionName));

// The native MT5 Manager API runs in the net48 OTS.Mt5Bridge process.
// This worker reaches it over localhost HTTP via a typed client.
var bridgeBaseUrl =
    builder.Configuration.GetSection(Mt5SyncOptions.SectionName)["BridgeBaseUrl"]
    ?? "http://127.0.0.1:5099";

builder.Services.AddHttpClient<IMt5SyncService, BridgeMt5SyncService>(client =>
{
    client.BaseAddress = new Uri(bridgeBaseUrl.TrimEnd('/') + "/");
    client.Timeout = TimeSpan.FromSeconds(60);
});

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
