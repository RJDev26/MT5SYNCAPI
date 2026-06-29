using OTS.UtilityService.Options;
using OTS.UtilityService.Services;

var builder = Host.CreateApplicationBuilder(args);

builder.Services
    .AddOptions<Mt5ManagerOptions>()
    .Bind(builder.Configuration.GetSection(Mt5ManagerOptions.SectionName))
    .ValidateDataAnnotations()
    .Validate(options => options.Port > 0 && options.Port <= 65535, "MT5 manager port must be between 1 and 65535.")
    .ValidateOnStart();

builder.Services.AddSingleton<IMt5ManagerClient, Mt5ManagerClient>();
builder.Services.AddHostedService<Mt5TradeSyncWorker>();

var host = builder.Build();
host.Run();
