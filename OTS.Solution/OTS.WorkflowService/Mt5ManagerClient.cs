using System.Collections;
using System.Reflection;
using Microsoft.Extensions.Options;

namespace OTS.WorkflowService;

public sealed class Mt5ManagerClient : IMt5ManagerClient
{
    private static readonly string[] ManagerApiAssemblies =
    [
        "MetaQuotes.MT5ManagerAPI64.dll",
        "MetaQuotes.MT5ManagerAPI64-net2.0.dll",
        "MetaQuotes.MT5ManagerAPI(64).dll",
        "MetaQuotes.MT5ManagerAPI.dll"
    ];

    private readonly Mt5ConnectionOptions _options;
    private readonly ILogger<Mt5ManagerClient> _logger;

    public Mt5ManagerClient(IOptions<Mt5ConnectionOptions> options, ILogger<Mt5ManagerClient> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public Task<Mt5SyncSnapshot> GetOrdersAndDealsAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ValidateOptions();

        var assembly = LoadManagerAssembly();
        var factoryType = assembly.GetType("MetaQuotes.MT5ManagerAPI.SMTManagerAPIFactory", throwOnError: true)!;
        object? manager = null;

        try
        {
            InvokeOptionalStatic(factoryType, "Initialize");
            manager = CreateManager(factoryType);
            Connect(manager);

            var orders = ReadOrders(manager).ToArray();
            var deals = ReadDeals(manager).ToArray();

            return Task.FromResult(new Mt5SyncSnapshot(
                DateTimeOffset.UtcNow,
                _options.ServerEndpoint,
                _options.Login,
                orders,
                deals));
        }
        finally
        {
            InvokeOptional(manager, "Disconnect");
            InvokeOptional(manager, "Release");
            InvokeOptionalStatic(factoryType, "Shutdown");
        }
    }

    private void ValidateOptions()
    {
        if (string.IsNullOrWhiteSpace(_options.Server))
        {
            throw new InvalidOperationException("MT5 server is not configured.");
        }

        if (_options.Login <= 0)
        {
            throw new InvalidOperationException("MT5 manager login is not configured.");
        }

        if (string.IsNullOrWhiteSpace(_options.Password))
        {
            throw new InvalidOperationException("MT5 manager password is not configured.");
        }
    }

    private Assembly LoadManagerAssembly()
    {
        var probeDirectories = BuildSdkProbeDirectories().Distinct(StringComparer.OrdinalIgnoreCase);
        foreach (var directory in probeDirectories)
        {
            foreach (var fileName in ManagerApiAssemblies)
            {
                var assemblyPath = Path.Combine(directory, fileName);
                if (File.Exists(assemblyPath))
                {
                    _logger.LogInformation("Loading MT5 Manager SDK from {AssemblyPath}.", assemblyPath);
                    return Assembly.LoadFrom(assemblyPath);
                }
            }
        }

        throw new FileNotFoundException(
            "MT5 Manager SDK was not found. Restore the MetaQuotes.MT5ManagerAPI64-net2.0 NuGet package or set Mt5:SdkDirectory to a folder that contains the official MetaQuotes MT5 Manager SDK DLLs.");
    }

    private IEnumerable<string> BuildSdkProbeDirectories()
    {
        yield return AppContext.BaseDirectory;

        if (!string.IsNullOrWhiteSpace(_options.SdkDirectory))
        {
            yield return Path.GetFullPath(_options.SdkDirectory);
        }

        var packageRoot = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            ".nuget",
            "packages",
            "metaquotes.mt5managerapi64-net2.0",
            "1755.0.0");

        if (Directory.Exists(packageRoot))
        {
            foreach (var directory in Directory.EnumerateDirectories(packageRoot, "*", SearchOption.AllDirectories))
            {
                yield return directory;
            }
        }
    }

    private static object CreateManager(Type factoryType)
    {
        var versionProperty = factoryType.GetProperty("ManagerAPIVersion", BindingFlags.Public | BindingFlags.Static)
            ?? throw new MissingMethodException(factoryType.FullName, "ManagerAPIVersion");
        var createManager = factoryType.GetMethods(BindingFlags.Public | BindingFlags.Static)
            .FirstOrDefault(m => m.Name == "CreateManager" && m.GetParameters().Length == 2)
            ?? throw new MissingMethodException(factoryType.FullName, "CreateManager");

        var args = new[] { versionProperty.GetValue(null), null };
        var manager = createManager.Invoke(null, args);
        EnsureOk(args[1], "CreateManager");

        return manager ?? throw new InvalidOperationException("MT5 Manager SDK returned a null manager instance.");
    }

    private void Connect(object manager)
    {
        var pumpMode = ResolveEnumValue(manager.GetType(), "EnPumpModes", "PUMP_MODE_FULL", "PUMP_MODE_USERS", "PUMP_MODE_NONE");
        var timeout = (uint)TimeSpan.FromSeconds(Math.Max(1, _options.ConnectTimeoutSeconds)).TotalMilliseconds;
        var result = InvokeRequired(manager, "Connect", _options.ServerEndpoint, (ulong)_options.Login, _options.Password, null, pumpMode, timeout);
        EnsureOk(result, "Connect");
    }

    private IEnumerable<Mt5OrderSnapshot> ReadOrders(object manager)
    {
        foreach (var methodName in new[] { "OrderGetAll", "OrdersGet", "OrderRequest" })
        {
            var rows = InvokeOptional(manager, methodName);
            if (rows is null || rows is bool boolResult && !boolResult)
            {
                continue;
            }

            foreach (var row in AsEnumerable(rows))
            {
                yield return new Mt5OrderSnapshot(
                    Convert.ToInt64(ReadMember(row, "Order", "OrderTicket", "Ticket") ?? 0),
                    Convert.ToInt64(ReadMember(row, "Login") ?? 0),
                    Convert.ToString(ReadMember(row, "Symbol")) ?? string.Empty,
                    Convert.ToString(ReadMember(row, "Type", "TypeName")) ?? string.Empty,
                    Convert.ToDouble(ReadMember(row, "VolumeCurrent", "Volume", "VolumeExt") ?? 0),
                    Convert.ToDouble(ReadMember(row, "PriceOrder", "PriceOpen", "Price") ?? 0),
                    ConvertToDateTimeOffset(ReadMember(row, "TimeSetup", "TimeCreate", "Time")));
            }

            yield break;
        }
    }

    private IEnumerable<Mt5DealSnapshot> ReadDeals(object manager)
    {
        if (_options.TradingLogins.Length == 0)
        {
            _logger.LogWarning("No MT5 trading logins are configured; deal history retrieval requires Mt5:TradingLogins.");
            yield break;
        }

        var dateTo = DateTime.UtcNow;
        var dateFrom = dateTo.AddDays(-Math.Max(1, _options.HistoryLookbackDays));

        foreach (var login in _options.TradingLogins)
        {
            var rows = InvokeOptional(manager, "DealRequest", login, dateFrom, dateTo)
                ?? InvokeOptional(manager, "DealsRequest", login, dateFrom, dateTo);
            if (rows is null || rows is bool boolResult && !boolResult)
            {
                continue;
            }

            foreach (var row in AsEnumerable(rows))
            {
                yield return new Mt5DealSnapshot(
                    Convert.ToInt64(ReadMember(row, "Deal", "DealTicket", "Ticket") ?? 0),
                    Convert.ToInt64(ReadMember(row, "Login") ?? login),
                    Convert.ToString(ReadMember(row, "Symbol")) ?? string.Empty,
                    Convert.ToString(ReadMember(row, "Action", "Type", "Entry")) ?? string.Empty,
                    Convert.ToDouble(ReadMember(row, "Volume", "VolumeExt") ?? 0),
                    Convert.ToDouble(ReadMember(row, "Price") ?? 0),
                    Convert.ToDouble(ReadMember(row, "Profit") ?? 0),
                    ConvertToDateTimeOffset(ReadMember(row, "Time", "TimeMsc")));
            }
        }
    }

    private static IEnumerable<object> AsEnumerable(object value)
    {
        if (value is string)
        {
            yield break;
        }

        if (value is IEnumerable enumerable)
        {
            foreach (var item in enumerable)
            {
                if (item is not null)
                {
                    yield return item;
                }
            }
        }
        else
        {
            yield return value;
        }
    }

    private static object? ReadMember(object instance, params string[] names)
    {
        var type = instance.GetType();
        foreach (var name in names)
        {
            var property = type.GetProperty(name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (property is not null)
            {
                return property.GetValue(instance);
            }

            var method = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase)
                .FirstOrDefault(m => m.Name.Equals(name, StringComparison.OrdinalIgnoreCase) && m.GetParameters().Length == 0);
            if (method is not null)
            {
                return method.Invoke(instance, null);
            }
        }

        return null;
    }

    private static DateTimeOffset ConvertToDateTimeOffset(object? value)
    {
        return value switch
        {
            DateTime dateTime => new DateTimeOffset(DateTime.SpecifyKind(dateTime, DateTimeKind.Utc)),
            DateTimeOffset dateTimeOffset => dateTimeOffset,
            long unixSeconds => DateTimeOffset.FromUnixTimeSeconds(unixSeconds),
            int unixSeconds => DateTimeOffset.FromUnixTimeSeconds(unixSeconds),
            uint unixSeconds => DateTimeOffset.FromUnixTimeSeconds(unixSeconds),
            ulong unixSeconds => DateTimeOffset.FromUnixTimeSeconds((long)unixSeconds),
            _ => DateTimeOffset.UtcNow
        };
    }

    private static object? ResolveEnumValue(Type managerType, string enumName, params string[] preferredNames)
    {
        var enumType = managerType.GetNestedType(enumName, BindingFlags.Public)
            ?? managerType.Assembly.GetTypes().FirstOrDefault(t => t.IsEnum && t.Name == enumName);
        if (enumType is null)
        {
            return null;
        }

        foreach (var preferredName in preferredNames)
        {
            if (Enum.IsDefined(enumType, preferredName))
            {
                return Enum.Parse(enumType, preferredName);
            }
        }

        return Enum.GetValues(enumType).GetValue(0);
    }

    private static object? InvokeOptionalStatic(Type type, string methodName)
        => type.GetMethods(BindingFlags.Public | BindingFlags.Static)
            .FirstOrDefault(m => m.Name == methodName && m.GetParameters().Length == 0)
            ?.Invoke(null, null);

    private static object? InvokeOptional(object? instance, string methodName, params object?[] args)
        => instance?.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .FirstOrDefault(m => m.Name == methodName && m.GetParameters().Length == args.Length)
            ?.Invoke(instance, args);

    private static object? InvokeRequired(object instance, string methodName, params object?[] args)
    {
        var method = instance.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .FirstOrDefault(m => m.Name == methodName && m.GetParameters().Length == args.Length)
            ?? throw new MissingMethodException(instance.GetType().FullName, methodName);

        return method.Invoke(instance, args);
    }

    private static void EnsureOk(object? result, string operation)
    {
        if (result is null)
        {
            return;
        }

        if (string.Equals(result.ToString(), "MT_RET_OK", StringComparison.OrdinalIgnoreCase) || string.Equals(result.ToString(), "0", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        throw new InvalidOperationException($"MT5 Manager SDK {operation} failed: {result}");
    }
}
