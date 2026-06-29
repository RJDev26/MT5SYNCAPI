using System.Reflection;
using System.Runtime.Loader;
using System.Text.Json;

namespace OTS.WorkflowService;

/// <summary>
/// Thin adapter over the MetaQuotes MT5 Manager .NET API.
///
/// The NuGet package exposes native-style classes (CMTManagerAPIFactory/CIMTManager)
/// and methods that often use return codes plus out parameters. Reflection keeps this
/// worker buildable in environments where the native Windows Manager API runtime is not
/// installed, while still calling the real Manager API methods at runtime.
/// </summary>
public sealed class Mt5ManagerClient : IDisposable
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = false };

    private readonly Mt5ManagerOptions _options;
    private readonly ILogger<Mt5ManagerClient> _logger;
    private object? _factory;
    private object? _manager;
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

        var assembly = LoadManagerAssembly();
        var factoryType = GetFactoryType(assembly);
        _factory = Activator.CreateInstance(factoryType) ?? factoryType;

        InitializeFactory(_factory);
        var version = ReadApiVersion(_factory);
        _manager = CreateManager(_factory, version);

        var connectResult = Invoke(
            _manager,
            "Connect",
            _options.Server,
            _options.Login,
            _options.Password,
            null,
            (ulong)_options.PumpingMode,
            _options.TimeoutMilliseconds);
        EnsureOk(connectResult, "Connect");

        _connected = true;
        _logger.LogInformation("Connected to MT5 Manager server {Server} as login {Login}.", _options.Server, _options.Login);
    }

    public Mt5Snapshot GetTradeAndOrderSnapshot()
    {
        Connect();
        ArgumentNullException.ThrowIfNull(_manager);

        var orders = ReadCurrentOrders(_manager).Take((int)_options.MaxRows).ToList();
        var deals = ReadDeals(_manager).Take((int)_options.MaxRows).ToList();

        return new Mt5Snapshot(orders, deals);
    }

    public void Dispose()
    {
        if (_manager is not null)
        {
            InvokeIfExists(_manager, "Disconnect");
            InvokeIfExists(_manager, "Release");
            (_manager as IDisposable)?.Dispose();
        }

        _manager = null;
        _connected = false;

        if (_factory is not null)
        {
            InvokeIfExists(_factory, "Shutdown");
        }

        _factory = null;
    }

    private IEnumerable<string> ReadCurrentOrders(object manager)
    {
        foreach (var row in ReadCollectionFromCreateArrayRequest(manager, "OrderCreateArray", "OrderRequestByGroup", _options.OrderGroupMask))
        {
            yield return row;
        }

        foreach (var row in ReadCollectionByTotalAndNext(manager, "OrderGetTotal", "OrderGetNext"))
        {
            yield return row;
        }
    }

    private IEnumerable<string> ReadDeals(object manager)
    {
        if (_options.DealHistoryLogin is null)
        {
            _logger.LogInformation("Mt5Manager:DealHistoryLogin is not configured, so deal history request is skipped.");
            yield break;
        }

        var to = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var from = DateTimeOffset.UtcNow.AddDays(-Math.Max(1, _options.DealHistoryDays)).ToUnixTimeSeconds();

        foreach (var row in ReadCollectionFromCreateArrayRequest(
                     manager,
                     "DealCreateArray",
                     "DealRequest",
                     _options.DealHistoryLogin.Value,
                     from,
                     to))
        {
            yield return row;
        }
    }

    private static IEnumerable<string> ReadCollectionFromCreateArrayRequest(object manager, string createArrayMethod, string requestMethod, params object?[] requestArguments)
    {
        var array = InvokeIfExists(manager, createArrayMethod);
        if (array is null)
        {
            yield break;
        }

        var arguments = requestArguments.Concat(new[] { array }).ToArray();
        EnsureOk(Invoke(manager, requestMethod, arguments), requestMethod);

        foreach (var row in ReadArrayLikeObject(array))
        {
            yield return row;
        }

        InvokeIfExists(array, "Release");
    }

    private static IEnumerable<string> ReadCollectionByTotalAndNext(object manager, string totalMethod, string nextMethod)
    {
        var totalObject = InvokeIfExists(manager, totalMethod);
        if (totalObject is null || !TryToUInt(totalObject, out var total))
        {
            yield break;
        }

        for (uint index = 0; index < total; index++)
        {
            var next = InvokeIfExists(manager, nextMethod, index);
            if (next is not null)
            {
                yield return SerializeObject(next);
            }
        }
    }

    private static IEnumerable<string> ReadArrayLikeObject(object array)
    {
        if (array is System.Collections.IEnumerable enumerable && array is not string)
        {
            foreach (var item in enumerable)
            {
                yield return SerializeObject(item);
            }

            yield break;
        }

        var totalObject = InvokeIfExists(array, "Total");
        if (totalObject is null || !TryToUInt(totalObject, out var total))
        {
            yield return SerializeObject(array);
            yield break;
        }

        for (uint index = 0; index < total; index++)
        {
            var item = InvokeIfExists(array, "Next", index) ?? InvokeIfExists(array, "Get", index);
            if (item is not null)
            {
                yield return SerializeObject(item);
            }
        }
    }

    private static void InitializeFactory(object factory)
    {
        var nativeDllPath = GetNativeManagerDllPath();
        var initializeResults = new[]
            {
                nativeDllPath is null ? null : InvokeIfExists(factory, "Initialize", nativeDllPath),
                InvokeIfExists(factory, "Initialize", (string?)null),
                InvokeIfExists(factory, "Initialize"),
                nativeDllPath is null ? null : InvokeIfExists(factory, "Init", nativeDllPath),
                InvokeIfExists(factory, "Init", (string?)null),
                InvokeIfExists(factory, "Init")
            }
            .Where(result => result is not null)
            .ToList();

        if (initializeResults.Count == 0 || initializeResults.Any(IsOkReturnCode))
        {
            return;
        }

        // The MetaQuotes NuGet package variants used in deployments can return
        // MT_RET_ERROR from Initialize when the native DLL is already loaded or when
        // initialization is not required. Continue to Version/CreateManager/Connect,
        // because those calls are the real validation for getting orders and deals
        // with manager credentials.
    }

    private static uint ReadApiVersion(object factory)
    {
        try
        {
            var invocation = InvokeWithOut(factory, "Version", typeof(uint));
            EnsureOk(invocation.ReturnValue, "Version");
            return invocation.OutValues.OfType<uint>().FirstOrDefault();
        }
        catch (MissingMethodException)
        {
            return ReadApiVersionConstant(factory) ?? 0;
        }
    }

    private static object CreateManager(object factory, uint version)
    {
        if (version > 0)
        {
            var manager = TryCreateManager(factory, version);
            if (manager is not null)
            {
                return manager;
            }
        }

        return TryCreateManager(factory)
            ?? TryCreateManagerWithoutOut(factory, version)
            ?? TryCreateManagerWithoutOut(factory)
            ?? throw new InvalidOperationException("Unable to create an MT5 manager instance from the MetaQuotes Manager API factory.");
    }

    private static object? TryCreateManager(object factory, params object?[] arguments)
    {
        try
        {
            var invocation = InvokeWithOut(factory, "CreateManager", typeof(object), arguments);
            EnsureOk(invocation.OutValues.FirstOrDefault(), "CreateManager");
            return invocation.ReturnValue;
        }
        catch
        {
            return null;
        }
    }

    private static object? TryCreateManagerWithoutOut(object factory, params object?[] arguments)
    {
        try
        {
            return Invoke(factory, "CreateManager", arguments);
        }
        catch
        {
            return null;
        }
    }

    private static uint? ReadApiVersionConstant(object factory)
    {
        var type = factory as Type ?? factory.GetType();
        var flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;

        foreach (var property in type.GetProperties(flags).Where(p => IsVersionMember(p.Name)))
        {
            if (TryConvertToUInt(property.GetValue(factory is Type ? null : factory), out var version))
            {
                return version;
            }
        }

        foreach (var field in type.GetFields(flags).Where(f => IsVersionMember(f.Name)))
        {
            if (TryConvertToUInt(field.GetValue(factory is Type ? null : factory), out var version))
            {
                return version;
            }
        }

        return null;
    }

    private static bool IsVersionMember(string name)
    {
        return name.Contains("Version", StringComparison.OrdinalIgnoreCase)
            && name.Contains("API", StringComparison.OrdinalIgnoreCase);
    }

    private static bool TryConvertToUInt(object? value, out uint result)
    {
        try
        {
            if (value is null)
            {
                result = 0;
                return false;
            }

            result = Convert.ToUInt32(value);
            return true;
        }
        catch
        {
            result = 0;
            return false;
        }
    }

    private static Assembly LoadManagerAssembly()
    {
        var loadedAssembly = AppDomain.CurrentDomain.GetAssemblies()
            .FirstOrDefault(a => a.GetName().Name?.Contains("MT5ManagerAPI", StringComparison.OrdinalIgnoreCase) == true);
        if (loadedAssembly is not null)
        {
            return loadedAssembly;
        }

        foreach (var assemblyPath in GetManagerAssemblyCandidates())
        {
            try
            {
                return AssemblyLoadContext.Default.LoadFromAssemblyPath(assemblyPath);
            }
            catch (FileLoadException)
            {
                return Assembly.LoadFrom(assemblyPath);
            }
            catch (BadImageFormatException)
            {
                // Ignore native helper DLLs; continue until the managed wrapper DLL is found.
            }
        }

        throw new FileNotFoundException(
            "Unable to locate the managed MT5 Manager API assembly. Ensure the MetaQuotes.MT5ManagerAPI64-net2.0 package DLLs are copied to the WorkflowService output folder.");
    }

    private static IEnumerable<string> GetManagerAssemblyCandidates()
    {
        var baseDirectory = AppContext.BaseDirectory;
        var candidateNames = new[]
        {
            "MetaQuotes.MT5ManagerAPI64.dll",
            "MetaQuotes.MT5ManagerAPI.dll",
            "MT5ManagerAPI64.dll",
            "MT5ManagerAPI.dll"
        };

        foreach (var candidateName in candidateNames)
        {
            var path = Path.Combine(baseDirectory, candidateName);
            if (File.Exists(path))
            {
                yield return path;
            }
        }

        foreach (var path in Directory.EnumerateFiles(baseDirectory, "*MT5*Manager*API*.dll", SearchOption.AllDirectories))
        {
            yield return path;
        }
    }


    private static string? GetNativeManagerDllPath()
    {
        var baseDirectory = AppContext.BaseDirectory;
        var candidateNames = new[]
        {
            "MT5APIManager64.dll",
            "MT5APIManager.dll"
        };

        foreach (var candidateName in candidateNames)
        {
            var path = Path.Combine(baseDirectory, candidateName);
            if (File.Exists(path))
            {
                return path;
            }
        }

        return Directory.EnumerateFiles(baseDirectory, "MT5APIManager*.dll", SearchOption.AllDirectories)
            .FirstOrDefault();
    }

    private static Type GetFactoryType(Assembly assembly)
    {
        return GetFactoryTypeOrDefault(assembly)
            ?? throw new InvalidOperationException("Unable to find CMTManagerAPIFactory in the MetaQuotes MT5 Manager API assembly.");
    }

    private static Type? GetFactoryTypeOrDefault(Assembly assembly)
    {
        var types = assembly.GetTypes();
        return types.FirstOrDefault(t => string.Equals(t.Name, "CMTManagerAPIFactory", StringComparison.OrdinalIgnoreCase) && HasMethod(t, "CreateManager"))
            ?? types.FirstOrDefault(t => t.Name.Contains("ManagerAPIFactory", StringComparison.OrdinalIgnoreCase) && HasMethod(t, "CreateManager"))
            ?? types.FirstOrDefault(t => t.Name.Contains("ManagerFactory", StringComparison.OrdinalIgnoreCase) && HasMethod(t, "CreateManager"));
    }

    private static bool HasMethod(Type type, string methodName)
    {
        return type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
            .Any(m => string.Equals(m.Name, methodName, StringComparison.OrdinalIgnoreCase));
    }

    private static object? InvokeIfExists(object target, string methodName, params object?[] arguments)
    {
        try
        {
            return Invoke(target, methodName, arguments);
        }
        catch (MissingMethodException)
        {
            return null;
        }
    }

    private static object? Invoke(object target, string methodName, params object?[] arguments)
    {
        var type = target as Type ?? target.GetType();
        var method = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
            .FirstOrDefault(m => string.Equals(m.Name, methodName, StringComparison.OrdinalIgnoreCase) && ParametersMatch(m.GetParameters(), arguments));

        if (method is null)
        {
            throw new MissingMethodException(type.FullName, methodName);
        }

        return method.Invoke(target is Type ? null : target, ConvertArguments(method.GetParameters(), arguments));
    }

    private static InvocationResult InvokeWithOut(object target, string methodName, Type outType, params object?[] inputArguments)
    {
        var type = target as Type ?? target.GetType();
        var method = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
            .FirstOrDefault(m => string.Equals(m.Name, methodName, StringComparison.OrdinalIgnoreCase)
                && m.GetParameters().Length == inputArguments.Length + 1
                && m.GetParameters().Last().ParameterType.IsByRef);

        if (method is null)
        {
            throw new MissingMethodException(type.FullName, methodName);
        }

        var outParameterType = method.GetParameters().Last().ParameterType.GetElementType() ?? outType;
        var arguments = inputArguments.Concat(new[] { GetDefault(outParameterType) }).ToArray();
        var invokeArguments = ConvertArguments(method.GetParameters(), arguments);
        var returnValue = method.Invoke(target is Type ? null : target, invokeArguments);
        return new InvocationResult(returnValue, invokeArguments.Skip(inputArguments.Length).ToArray());
    }


    private static object?[] ConvertArguments(ParameterInfo[] parameters, object?[] arguments)
    {
        var converted = new object?[arguments.Length];
        for (var i = 0; i < arguments.Length; i++)
        {
            if (arguments[i] is null || parameters[i].ParameterType.IsByRef)
            {
                converted[i] = arguments[i];
                continue;
            }

            var parameterType = Nullable.GetUnderlyingType(parameters[i].ParameterType) ?? parameters[i].ParameterType;
            converted[i] = parameterType.IsInstanceOfType(arguments[i])
                ? arguments[i]
                : Convert.ChangeType(arguments[i], parameterType, System.Globalization.CultureInfo.InvariantCulture);
        }

        return converted;
    }

    private static bool ParametersMatch(ParameterInfo[] parameters, object?[] arguments)
    {
        if (parameters.Length != arguments.Length)
        {
            return false;
        }

        for (var i = 0; i < parameters.Length; i++)
        {
            if (arguments[i] is null || parameters[i].ParameterType.IsByRef)
            {
                continue;
            }

            var parameterType = Nullable.GetUnderlyingType(parameters[i].ParameterType) ?? parameters[i].ParameterType;
            if (!parameterType.IsInstanceOfType(arguments[i]) && !CanConvert(arguments[i]!, parameterType))
            {
                return false;
            }
        }

        return true;
    }

    private static bool CanConvert(object value, Type type)
    {
        return value is IConvertible && typeof(IConvertible).IsAssignableFrom(type);
    }


    private static bool IsOkReturnCode(object? returnValue)
    {
        if (returnValue is null)
        {
            return true;
        }

        if (returnValue is bool boolResult)
        {
            return boolResult;
        }

        if (returnValue is IConvertible convertible)
        {
            return convertible.ToInt64(System.Globalization.CultureInfo.InvariantCulture) == 0;
        }

        return true;
    }

    private static void EnsureOk(object? returnValue, string operation)
    {
        if (returnValue is null)
        {
            return;
        }

        if (returnValue is bool boolResult)
        {
            if (!boolResult)
            {
                throw new InvalidOperationException($"MT5 Manager API {operation} returned false.");
            }

            return;
        }

        if (returnValue is IConvertible convertible)
        {
            var code = convertible.ToInt64(System.Globalization.CultureInfo.InvariantCulture);
            if (code != 0)
            {
                throw new InvalidOperationException($"MT5 Manager API {operation} failed with return code {returnValue}.");
            }
        }
    }

    private static bool TryToUInt(object value, out uint result)
    {
        try
        {
            result = Convert.ToUInt32(value);
            return true;
        }
        catch
        {
            result = 0;
            return false;
        }
    }

    private static object? GetDefault(Type type)
    {
        return type.IsValueType ? Activator.CreateInstance(type) : null;
    }

    private static string SerializeObject(object? value)
    {
        if (value is null)
        {
            return "null";
        }

        var members = value.GetType()
            .GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.GetParameters().Length == 0 && m.ReturnType != typeof(void) && !m.IsSpecialName && m.Name != "GetHashCode")
            .ToDictionary(m => m.Name, m => SafeInvoke(value, m));

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

    private sealed record InvocationResult(object? ReturnValue, object?[] OutValues);
}

public sealed record Mt5Snapshot(IReadOnlyList<string> Orders, IReadOnlyList<string> Deals);
