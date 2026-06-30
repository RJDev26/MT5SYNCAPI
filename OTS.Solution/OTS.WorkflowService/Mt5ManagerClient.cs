using System.Reflection;
using System.Text.Json;
using MetaQuotes.MT5CommonAPI;
using MetaQuotes.MT5ManagerAPI;

namespace OTS.WorkflowService;

public sealed class Mt5ManagerClient : IDisposable
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = false };

    private readonly Mt5ManagerOptions _options;
    private readonly ILogger<Mt5ManagerClient> _logger;
    private CIMTManagerAPI? _manager;
    private bool _connected;

    public Mt5ManagerClient(Mt5ManagerOptions options, ILogger<Mt5ManagerClient> logger)
    {
        _options = options;
        _logger = logger;
    }

    public void Connect()
    {
        if (_connected)
        {
            return;
        }

        var result = InitializeManagerApi(_options.NativeLibraryPath);
        ThrowIfFailed(result, "Initialize MT5 Manager API. If this fails, set Mt5Manager:NativeLibraryPath to the folder or DLL path that contains MT5APIManager64.dll");

        _manager = SMTManagerAPIFactory.CreateManager(SMTManagerAPIFactory.ManagerAPIVersion, out result);
        ThrowIfFailed(result, "Create MT5 manager instance");

        if (_manager is null)
        {
            throw new InvalidOperationException("MT5 Manager API returned MT_RET_OK but did not create a manager instance.");
        }

        result = _manager.Connect(
            _options.Server,
            _options.Login,
            _options.Password,
            null,
            CIMTManagerAPI.EnPumpModes.PUMP_MODE_FULL,
            _options.TimeoutMilliseconds);
        ThrowIfFailed(result, $"Connect to MT5 server {_options.Server} as manager {_options.Login}");

        _connected = true;
        _logger.LogInformation("Connected to MT5 Manager server {Server} as manager login {Login}.", _options.Server, _options.Login);
    }


    private static MTRetCode InitializeManagerApi(string? configuredNativeLibraryPath)
    {
        var nativeDllPath = GetNativeManagerDllPath();
        var candidates = new List<string?>();

        if (!string.IsNullOrWhiteSpace(configuredNativeLibraryPath))
        {
            candidates.Add(configuredNativeLibraryPath);
        }

        if (!string.IsNullOrWhiteSpace(nativeDllPath))
        {
            candidates.Add(nativeDllPath);

            var nativeDirectory = Path.GetDirectoryName(nativeDllPath);
            if (!string.IsNullOrWhiteSpace(nativeDirectory))
            {
                candidates.Add(nativeDirectory);
            }
        }

        candidates.Add(AppContext.BaseDirectory);
        candidates.Add(string.Empty);
        candidates.Add(null);

        MTRetCode lastResult = MTRetCode.MT_RET_ERROR;
        foreach (var candidate in candidates.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            lastResult = SMTManagerAPIFactory.Initialize(candidate);
            if (lastResult == MTRetCode.MT_RET_OK)
            {
                return lastResult;
            }
        }

        return lastResult;
    }

    private static string? GetNativeManagerDllPath()
    {
        var baseDirectory = AppContext.BaseDirectory;
        foreach (var fileName in new[] { "MT5APIManager64.dll", "MT5APIManager.dll" })
        {
            var path = Path.Combine(baseDirectory, fileName);
            if (File.Exists(path))
            {
                return path;
            }
        }

        return Directory.EnumerateFiles(baseDirectory, "MT5APIManager*.dll", SearchOption.AllDirectories)
            .FirstOrDefault();
    }

    public Mt5Snapshot GetTradeAndOrderSnapshot()
    {
        if (TryGetSnapshotFromMetaTrader5NetApi(out var terminalSnapshot))
        {
            return terminalSnapshot;
        }

        Connect();
        ArgumentNullException.ThrowIfNull(_manager);

        var orders = ReadOrders(_manager).Take((int)_options.MaxRows).ToList();
        var deals = ReadDeals(_manager).Take((int)_options.MaxRows).ToList();

        return new Mt5Snapshot(orders, deals);
    }

    public void Dispose()
    {
        if (_manager is not null)
        {
            _manager.Disconnect();
            _manager.Dispose();
            _manager = null;
        }

        _connected = false;
        SMTManagerAPIFactory.Shutdown();
    }


    private bool TryGetSnapshotFromMetaTrader5NetApi(out Mt5Snapshot snapshot)
    {
        snapshot = new Mt5Snapshot(Array.Empty<string>(), Array.Empty<string>());

        var assembly = AppDomain.CurrentDomain.GetAssemblies()
            .FirstOrDefault(a => a.GetName().Name?.Contains("MetaTrader5", StringComparison.OrdinalIgnoreCase) == true)
            ?? TryLoadAssembly("MetaTrader5.Net.API")
            ?? TryLoadAssembly("MetaTrader5.Net");

        if (assembly is null)
        {
            return false;
        }

        foreach (var apiType in assembly.GetTypes().Where(t => !t.IsAbstract && !t.IsInterface && t.GetConstructor(Type.EmptyTypes) is not null))
        {
            var api = Activator.CreateInstance(apiType);
            if (api is null)
            {
                continue;
            }

            if (!TryConnectMetaTrader5NetApi(api))
            {
                continue;
            }

            var from = DateTime.UtcNow.AddDays(-Math.Max(1, _options.DealHistoryDays));
            var to = DateTime.UtcNow;
            var orders = InvokeFirstDataMethod(api, new[] { "OrdersGet", "Orders", "GetOrders", "PositionsGet", "Positions", "GetPositions" }, Array.Empty<object?>(), new object?[] { _options.OrderGroupMask });
            var deals = InvokeFirstDataMethod(api, new[] { "HistoryDealsGet", "DealsGet", "GetDeals", "HistoryOrdersGet", "GetHistoryOrders" }, new object?[] { from, to }, new object?[] { from, to, _options.OrderGroupMask });

            snapshot = new Mt5Snapshot(NormalizeRows(orders).Take((int)_options.MaxRows).ToList(), NormalizeRows(deals).Take((int)_options.MaxRows).ToList());
            return true;
        }

        return false;
    }

    private bool TryConnectMetaTrader5NetApi(object api)
    {
        var login = Convert.ToInt64(_options.Login);
        return IsSuccess(InvokeFirstDataMethod(api, new[] { "Initialize", "Init", "Connect", "Login" },
                new object?[] { _options.Server, login, _options.Password },
                new object?[] { login, _options.Password, _options.Server },
                new object?[] { _options.Server, _options.Login, _options.Password },
                Array.Empty<object?>()))
            || IsSuccess(InvokeFirstDataMethod(api, new[] { "Login", "Connect" },
                new object?[] { login, _options.Password, _options.Server },
                new object?[] { _options.Server, login, _options.Password }));
    }

    private static Assembly? TryLoadAssembly(string name)
    {
        try
        {
            return Assembly.Load(name);
        }
        catch
        {
            return null;
        }
    }

    private static object? InvokeFirstDataMethod(object target, IEnumerable<string> methodNames, params object?[][] argumentSets)
    {
        foreach (var methodName in methodNames)
        {
            foreach (var arguments in argumentSets)
            {
                var method = target.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
                    .FirstOrDefault(candidate => string.Equals(candidate.Name, methodName, StringComparison.OrdinalIgnoreCase)
                        && ParametersMatch(candidate.GetParameters(), arguments));
                if (method is null)
                {
                    continue;
                }

                try
                {
                    return method.Invoke(target, ConvertArguments(method.GetParameters(), arguments!));
                }
                catch
                {
                    // Try next overload/name.
                }
            }
        }

        return null;
    }

    private static bool IsSuccess(object? result)
    {
        return result is null || result is true || (result is not bool && result is IConvertible convertible && convertible.ToInt64(System.Globalization.CultureInfo.InvariantCulture) == 0);
    }

    private static IEnumerable<string> NormalizeRows(object? value)
    {
        if (value is null)
        {
            return Array.Empty<string>();
        }

        if (value is System.Collections.IEnumerable enumerable && value is not string)
        {
            return enumerable.Cast<object?>().Where(item => item is not null).Select(item => SerializeObject(item!));
        }

        return new[] { SerializeObject(value) };
    }

    private IEnumerable<string> ReadOrders(CIMTManagerAPI manager)
    {
        var orders = manager.OrderCreateArray();
        if (orders is null)
        {
            throw new InvalidOperationException("MT5 Manager API could not create an order array.");
        }

        try
        {
            var result = InvokeMt5Method(manager, new[] { "OrderRequestByGroup", "OrderGetByGroup", "OrderGet" }, _options.OrderGroupMask, orders);
            ThrowIfFailed(result, $"Request MT5 open orders for group mask '{_options.OrderGroupMask}'");

            foreach (var item in ReadArrayItems(orders))
            {
                yield return item;
            }
        }
        finally
        {
            orders.Release();
        }
    }

    private IEnumerable<string> ReadDeals(CIMTManagerAPI manager)
    {
        var deals = manager.DealCreateArray();
        if (deals is null)
        {
            throw new InvalidOperationException("MT5 Manager API could not create a deal array.");
        }

        try
        {
            var to = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var from = DateTimeOffset.UtcNow.AddDays(-Math.Max(1, _options.DealHistoryDays)).ToUnixTimeSeconds();
            var result = _options.DealHistoryLogin is null
                ? InvokeMt5Method(manager, new[] { "DealRequestByGroup", "DealGetByGroup", "DealRequest", "DealGet" }, _options.OrderGroupMask, from, to, deals)
                : InvokeMt5Method(manager, new[] { "DealRequest", "DealGet" }, _options.DealHistoryLogin.Value, from, to, deals);

            ThrowIfFailed(result, _options.DealHistoryLogin is null
                ? $"Request MT5 deals for group mask '{_options.OrderGroupMask}'"
                : $"Request MT5 deals for login {_options.DealHistoryLogin.Value}");

            foreach (var item in ReadArrayItems(deals))
            {
                yield return item;
            }
        }
        finally
        {
            deals.Release();
        }
    }


    private static MTRetCode InvokeMt5Method(object target, IEnumerable<string> methodNames, params object[] arguments)
    {
        foreach (var methodName in methodNames)
        {
            var method = target.GetType()
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .FirstOrDefault(candidate => string.Equals(candidate.Name, methodName, StringComparison.OrdinalIgnoreCase)
                    && ParametersMatch(candidate.GetParameters(), arguments));

            if (method is null)
            {
                continue;
            }

            try
            {
                var result = method.Invoke(target, ConvertArguments(method.GetParameters(), arguments));
                return ToMtRetCode(result);
            }
            catch
            {
                // Try the next candidate overload/name.
            }
        }

        throw new InvalidOperationException($"None of the MT5 Manager API methods were found: {string.Join(", ", methodNames)}.");
    }

    private static bool ParametersMatch(ParameterInfo[] parameters, object?[] arguments)
    {
        if (parameters.Length != arguments.Length)
        {
            return false;
        }

        for (var i = 0; i < parameters.Length; i++)
        {
            if (arguments[i] is null)
            {
                continue;
            }

            var parameterType = Nullable.GetUnderlyingType(parameters[i].ParameterType) ?? parameters[i].ParameterType;
            if (!parameterType.IsInstanceOfType(arguments[i]) && !(arguments[i] is IConvertible && typeof(IConvertible).IsAssignableFrom(parameterType)))
            {
                return false;
            }
        }

        return true;
    }

    private static object?[] ConvertArguments(ParameterInfo[] parameters, object?[] arguments)
    {
        var converted = new object?[arguments.Length];
        for (var i = 0; i < arguments.Length; i++)
        {
            if (arguments[i] is null)
            {
                converted[i] = null;
                continue;
            }

            var parameterType = Nullable.GetUnderlyingType(parameters[i].ParameterType) ?? parameters[i].ParameterType;
            converted[i] = parameterType.IsInstanceOfType(arguments[i])
                ? arguments[i]
                : Convert.ChangeType(arguments[i], parameterType, System.Globalization.CultureInfo.InvariantCulture);
        }

        return converted;
    }

    private static MTRetCode ToMtRetCode(object? result)
    {
        if (result is MTRetCode retCode)
        {
            return retCode;
        }

        return (MTRetCode)Convert.ToUInt32(result ?? MTRetCode.MT_RET_OK);
    }

    private static IEnumerable<string> ReadArrayItems(object array)
    {
        var total = Convert.ToUInt32(array.GetType().GetMethod("Total")?.Invoke(array, null) ?? 0u);
        var nextMethod = array.GetType().GetMethod("Next") ?? array.GetType().GetMethod("Get");
        if (nextMethod is null)
        {
            yield break;
        }

        for (uint index = 0; index < total; index++)
        {
            var item = nextMethod.Invoke(array, new object[] { index });
            if (item is not null)
            {
                yield return SerializeObject(item);
            }
        }
    }

    private static void ThrowIfFailed(MTRetCode result, string operation)
    {
        if (result != MTRetCode.MT_RET_OK)
        {
            throw new InvalidOperationException($"{operation} failed with MT5 return code {result}.");
        }
    }

    private static string SerializeObject(object value)
    {
        var members = value.GetType()
            .GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(method => method.GetParameters().Length == 0 && method.ReturnType != typeof(void) && !method.IsSpecialName && method.Name != nameof(GetHashCode))
            .ToDictionary(method => method.Name, method => SafeInvoke(value, method));

        return members.Count == 0
            ? JsonSerializer.Serialize(value, value.GetType(), JsonOptions)
            : JsonSerializer.Serialize(members, JsonOptions);
    }

    private static object? SafeInvoke(object target, MethodInfo method)
    {
        try
        {
            return method.Invoke(target, null);
        }
        catch
        {
            return null;
        }
    }
}

public sealed record Mt5Snapshot(IReadOnlyList<string> Orders, IReadOnlyList<string> Deals);
