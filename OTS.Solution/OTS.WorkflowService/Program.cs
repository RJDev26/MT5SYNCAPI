using OTS.WorkflowService;
using OTS.WorkflowService.Options;
using OTS.WorkflowService.Services;

var builder = Host.CreateApplicationBuilder(args);

// Bind MT5 sync configuration from the "Mt5Sync" section.
builder.Services.Configure<Mt5SyncOptions>(
    builder.Configuration.GetSection(Mt5SyncOptions.SectionName));

// Register the MT5 data source. Swap StubMt5SyncService for the real
// Manager API implementation once the interop layer is available.
builder.Services.AddSingleton<IMt5SyncService, StubMt5SyncService>();

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
