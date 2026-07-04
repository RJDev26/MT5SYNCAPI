using Ots.WorkFlowService;
using Ots.WorkFlowService.MetaTrader;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.Configure<MetaTraderManagerOptions>(builder.Configuration.GetSection(MetaTraderManagerOptions.SectionName));
builder.Services.AddSingleton<IMetaTraderManagerClient, MetaTraderManagerClient>();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
