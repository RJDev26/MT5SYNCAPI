using Ots.WorkFlowService;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.Configure<Mt5ManagerOptions>(
    builder.Configuration.GetSection(Mt5ManagerOptions.SectionName));
builder.Services.AddSingleton<Mt5ManagerUtility>();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
