using System.Collections;
using System.Reflection;
using Microsoft.Extensions.Options;

namespace Ots.WorkFlowService.MetaTrader;

public sealed class MetaTraderManagerClient : IMetaTraderManagerClient, IDisposable
{
    private readonly MetaTraderManagerOptions _options;
    private readonly ILogger<MetaTraderManagerClient> _logger;
    private object? _manager;
    private bool _connected;

    public MetaTraderManagerClient(IOptions<MetaTraderManagerOptions> options, ILogger<MetaTraderManagerClient> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public Task<MetaTraderSnapshot> GetLiveDealsAndOrdersAsync(DateTime from, DateTime to, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        EnsureConnected();

        var fromUnix = new DateTimeOffset(from).ToUnixTimeSeconds();
        var toUnix = new DateTimeOffset(to).ToUnixTimeSeconds();
        var deals = ReadCollection<MetaTraderDeal>(["DealRequest", "DealRequestPage", "DealsRequest"], fromUnix, toUnix, MapDeal)
            .Take(_options.MaxRowsPerPoll)
            .ToList();
        var orders = ReadCollection<MetaTraderOrder>(["OrderRequest", "OrderRequestPage", "OrdersRequest"], fromUnix, toUnix, MapOrder)
            .Take(_options.MaxRowsPerPoll)
            .ToList();

        return Task.FromResult(new MetaTraderSnapshot(from, to, deals, orders));
    }

    public void Dispose()
    {
        if (_manager is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }

    private void EnsureConnected()
    {
        if (_connected && _manager is not null)
        {
            return;
        }

        _manager = CreateManager();
        InvokeBestEffort(_manager, "Connect", _options.Server, _options.Login, _options.Password, _options.PumpingMode, _options.TimeoutMilliseconds);
        _connected = true;
        _logger.LogInformation("Connected to MT5 manager server {Server} as login {Login} ({ManagerName})", _options.Server, _options.Login, _options.ManagerName);
    }

    private object CreateManager()
    {
        foreach (var assembly in LoadCandidateAssemblies())
        {
            try
            {
                var factoryType = assembly.GetTypes().FirstOrDefault(t =>
                    t.Name.Contains("ManagerAPIFactory", StringComparison.OrdinalIgnoreCase) ||
                    t.Name.Contains("MTManagerAPIFactory", StringComparison.OrdinalIgnoreCase));
                if (factoryType is not null)
                {
                    InvokeBestEffort(factoryType, "Initialize", AppContext.BaseDirectory);
                    InvokeBestEffort(factoryType, "Initialize");

                    var manager = InvokeBestEffort(factoryType, "CreateManager") ?? InvokeBestEffort(factoryType, "Create");
                    if (manager is not null)
                    {
                        return manager;
                    }
                }

                var managerType = assembly.GetTypes().FirstOrDefault(t =>
                    (t.Name.Equals("CManagerApi", StringComparison.OrdinalIgnoreCase) ||
                     t.Name.Contains("ManagerAPI", StringComparison.OrdinalIgnoreCase)) &&
                    !t.IsAbstract &&
                    t.GetConstructor(Type.EmptyTypes) is not null);
                if (managerType is not null && Activator.CreateInstance(managerType) is { } instance)
                {
                    return instance;
                }
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Unable to create MT5 manager from assembly {AssemblyName}", assembly.FullName);
            }
        }

        throw new InvalidOperationException("MT5 Manager API assembly was not found. Copy MetaQuotes.MT5ManagerAPI(64).dll to the Ots.WorkFlowService output folder or configure MetaTraderManager:AssemblyPaths with the full DLL path.");
    }

    private IEnumerable<Assembly> LoadCandidateAssemblies()
    {
        foreach (var assemblyPath in ResolveAssemblyPaths())
        {
            if (!File.Exists(assemblyPath))
            {
                continue;
            }

            Assembly assembly;
            try
            {
                assembly = Assembly.LoadFrom(assemblyPath);
                _logger.LogInformation("Loaded MT5 Manager API assembly from {AssemblyPath}", assemblyPath);
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Unable to load MT5 Manager API assembly from {AssemblyPath}", assemblyPath);
                continue;
            }

            yield return assembly;
        }

        foreach (var assemblyName in _options.AssemblyNames)
        {
            Assembly assembly;
            try
            {
                assembly = Assembly.Load(assemblyName);
                _logger.LogInformation("Loaded MT5 Manager API assembly by name {AssemblyName}", assemblyName);
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Unable to load MT5 Manager API assembly by name {AssemblyName}", assemblyName);
                continue;
            }

            yield return assembly;
        }
    }

    private IEnumerable<string> ResolveAssemblyPaths()
    {
        foreach (var path in _options.AssemblyPaths.Where(path => !string.IsNullOrWhiteSpace(path)))
        {
            yield return Path.GetFullPath(path, AppContext.BaseDirectory);
            yield return Path.GetFullPath(path, Directory.GetCurrentDirectory());
        }
    }

    private IReadOnlyList<T> ReadCollection<T>(string[] methodNames, long fromUnix, long toUnix, Func<object, T> mapper)
    {
        if (_manager is null)
        {
            return [];
        }

        foreach (var methodName in methodNames)
        {
            try
            {
                var result = InvokeBestEffort(_manager, methodName, fromUnix, toUnix, 0, _options.MaxRowsPerPoll)
                    ?? InvokeBestEffort(_manager, methodName, fromUnix, toUnix);
                if (result is null)
                {
                    continue;
                }

                return ToEnumerable(result).Select(mapper).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "MT5 manager method {MethodName} did not return {ItemType} rows", methodName, typeof(T).Name);
            }
        }

        _logger.LogWarning("No compatible MT5 manager method was found for {ItemType}", typeof(T).Name);
        return [];
    }

    private static IEnumerable<object> ToEnumerable(object value)
    {
        if (value is IEnumerable enumerable and not string)
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

    private static object? InvokeBestEffort(object target, string methodName, params object?[] supplied)
    {
        var type = target as Type ?? target.GetType();
        var flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;
        foreach (var method in type.GetMethods(flags).Where(m => m.Name.Equals(methodName, StringComparison.OrdinalIgnoreCase)).OrderByDescending(m => m.GetParameters().Length))
        {
            var args = BuildArguments(method.GetParameters(), supplied);
            if (args is null)
            {
                continue;
            }

            var result = method.Invoke(target is Type ? null : target, args);
            return result ?? args.FirstOrDefault(a => a is not null && a.GetType().Name.Contains("ManagerAPI", StringComparison.OrdinalIgnoreCase));
        }

        return null;
    }

    private static object?[]? BuildArguments(ParameterInfo[] parameters, object?[] supplied)
    {
        var args = new object?[parameters.Length];
        var suppliedIndex = 0;
        for (var i = 0; i < parameters.Length; i++)
        {
            var parameter = parameters[i];
            if (parameter.IsOut)
            {
                args[i] = parameter.ParameterType.GetElementType()?.IsValueType == true ? Activator.CreateInstance(parameter.ParameterType.GetElementType()!) : null;
                continue;
            }

            if (suppliedIndex < supplied.Length && TryConvert(supplied[suppliedIndex], parameter.ParameterType, out var converted))
            {
                args[i] = converted;
                suppliedIndex++;
                continue;
            }

            if (parameter.HasDefaultValue)
            {
                args[i] = parameter.DefaultValue;
                continue;
            }

            return null;
        }

        return args;
    }

    private static bool TryConvert(object? value, Type targetType, out object? converted)
    {
        converted = null;
        var actualType = Nullable.GetUnderlyingType(targetType) ?? targetType;
        if (value is null)
        {
            return !actualType.IsValueType;
        }

        if (actualType.IsInstanceOfType(value))
        {
            converted = value;
            return true;
        }

        try
        {
            converted = actualType.IsEnum ? Enum.ToObject(actualType, value) : Convert.ChangeType(value, actualType);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static MetaTraderDeal MapDeal(object deal) => new(
        GetUInt64(deal, "Deal", "DealId"),
        GetUInt64(deal, "Order", "OrderId"),
        GetUInt64(deal, "Login"),
        GetString(deal, "Symbol"),
        GetString(deal, "Action", "Entry"),
        GetDouble(deal, "Volume", "VolumeExt") / VolumeDivisor(deal),
        GetDouble(deal, "Price"),
        GetDouble(deal, "Profit"),
        GetDateTime(deal, "Time", "TimeMsc"));

    private static MetaTraderOrder MapOrder(object order) => new(
        GetUInt64(order, "Order", "OrderId"),
        GetUInt64(order, "Login"),
        GetString(order, "Symbol"),
        GetString(order, "Type"),
        GetString(order, "State"),
        GetDouble(order, "VolumeInitial", "VolumeInitialExt") / VolumeDivisor(order),
        GetDouble(order, "VolumeCurrent", "VolumeCurrentExt") / VolumeDivisor(order),
        GetDouble(order, "PriceOpen", "PriceOrder"),
        GetDateTime(order, "TimeSetup", "TimeSetupMsc"));

    private static double VolumeDivisor(object value) => HasMember(value, "VolumeExt") || HasMember(value, "VolumeInitialExt") ? 10_000_000d : 1d;
    private static bool HasMember(object value, string name) => value.GetType().GetMember(name, BindingFlags.Public | BindingFlags.Instance).Length > 0;
    private static string GetString(object value, params string[] names) => GetValue(value, names)?.ToString() ?? string.Empty;
    private static ulong GetUInt64(object value, params string[] names) => Convert.ToUInt64(GetValue(value, names) ?? 0);
    private static double GetDouble(object value, params string[] names) => Convert.ToDouble(GetValue(value, names) ?? 0);

    private static DateTime GetDateTime(object value, params string[] names)
    {
        var raw = GetValue(value, names);
        if (raw is DateTime dateTime) return dateTime;
        var unix = Convert.ToInt64(raw ?? 0);
        if (unix > 99_999_999_999) unix /= 1000;
        return unix <= 0 ? DateTime.MinValue : DateTimeOffset.FromUnixTimeSeconds(unix).UtcDateTime;
    }

    private static object? GetValue(object value, params string[] names)
    {
        foreach (var name in names)
        {
            var type = value.GetType();
            var property = type.GetProperty(name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (property is not null) return property.GetValue(value);
            var method = type.GetMethod(name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase, binder: null, types: Type.EmptyTypes, modifiers: null);
            if (method is not null) return method.Invoke(value, null);
        }

        return null;
    }
}
