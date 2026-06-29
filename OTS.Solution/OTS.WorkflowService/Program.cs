using OTS.WorkflowService;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<Mt5ConnectionOptions>(builder.Configuration.GetSection(Mt5ConnectionOptions.SectionName));
builder.Services.AddSingleton<IMt5ManagerClient, Mt5ManagerClient>();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
