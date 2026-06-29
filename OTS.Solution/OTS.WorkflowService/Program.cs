using OTS.WorkflowService;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.Configure<Mt5ManagerOptions>(builder.Configuration.GetSection("Mt5Manager"));
builder.Services.AddSingleton(provider => provider.GetRequiredService<Microsoft.Extensions.Options.IOptions<Mt5ManagerOptions>>().Value);
builder.Services.AddSingleton<Mt5ManagerClient>();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
