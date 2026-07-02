using System.Reflection;
using Microsoft.Extensions.Options;

namespace Ots.WorkFlowService.MetaTrader;

public sealed class MetaTraderManagerClient : IMetaTraderManagerClient
{
    private static readonly string[] ManagerTypeNames =
    [
        "MT5Manager",
        "MT5ManagerApiNet.MT5Manager, MT5ManagerApiNet",
        "MT5ManagerApiNet.MT5Manager",
        "mt5api.MT5Manager, mt5api",
        "mt5api.MT5Manager"
    ];

    private readonly MetaTraderManagerOptions _options;
    private readonly ILogger<MetaTraderManagerClient> _logger;

    public MetaTraderManagerClient(
        IOptions<MetaTraderManagerOptions> options,
        ILogger<MetaTraderManagerClient> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task<bool> CheckLoginAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return await Task.Run(() =>
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                using var manager = CreateManager();
                Login(manager.Instance);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Unable to connect to MT5 manager {ManagerName} ({ManagerLogin}) on {Server}.",
                    _options.ManagerName,
                    _options.ManagerLogin,
                    _options.Server);

                return false;
            }
        }, cancellationToken);
    }

    public async Task<MetaTraderSnapshot> GetDealsAndOrdersAsync(
        DateTime fromUtc,
        DateTime toUtc,
        ulong tradingLogin,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return await Task.Run(() =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            using var manager = CreateManager();
            Login(manager.Instance);

            var from = ToManagerTimestamp(fromUtc);
            var to = ToManagerTimestamp(toUtc);

            _logger.LogInformation(
                "Requesting MT5 deals and orders for login {TradingLogin} from {FromUtc:o} to {ToUtc:o}.",
                tradingLogin,
                fromUtc,
                toUtc);

            var pendingOrders = Invoke(manager.Instance, "HistoryRequest", tradingLogin, from, to);
            var deals = Invoke(manager.Instance, "DealRequest", tradingLogin, from, to);
            var orders = Invoke(manager.Instance, "DownloadOrderHistory", tradingLogin, fromUtc, toUtc);

            return new MetaTraderSnapshot(fromUtc, toUtc, tradingLogin, deals, orders, pendingOrders);
        }, cancellationToken);
    }

    private ManagerInstance CreateManager()
    {
        LoadKnownManagerAssemblies();

        foreach (var typeName in ManagerTypeNames)
        {
            var type = Type.GetType(typeName, throwOnError: false);
            if (type is null)
            {
                type = AppDomain.CurrentDomain
                    .GetAssemblies()
                    .Select(assembly => assembly.GetType(typeName, throwOnError: false))
                    .FirstOrDefault(candidate => candidate is not null);
            }

            if (type is null)
            {
                continue;
            }

            var instance = Activator.CreateInstance(type)
                ?? throw new InvalidOperationException($"Unable to create an instance of {type.FullName}.");

            return new ManagerInstance(instance);
        }

        throw new InvalidOperationException(
            "Unable to locate the MT5Manager type. Verify that the MetaTrader manager assemblies are copied to the Ots.WorkFlowService output folder.");
    }

    private static void LoadKnownManagerAssemblies()
    {
        foreach (var assemblyName in new[] { "MT5ManagerApiNet", "mt5api", "MetaQuotes.MT5ManagerAPI64", "MetaQuotes.MT5CommonAPI64" })
        {
            try
            {
                Assembly.Load(assemblyName);
            }
            catch
            {
                // The package may expose only one of these assemblies; type discovery below will report a clear error if none load.
            }
        }
    }

    private void Login(object manager)
    {
        if (!TryInvoke(
                manager,
                "Login",
                out var loginResult,
                _options.Server,
                _options.ManagerLogin,
                _options.Password,
                _options.ConnectionTimeoutMilliseconds)
            && !TryInvoke(
                manager,
                "Login",
                out loginResult,
                _options.Server,
                _options.ManagerLogin,
                _options.Password)
            && !TryInvoke(
                manager,
                "Connect",
                out loginResult,
                _options.Server,
                _options.ManagerLogin,
                _options.Password,
                _options.ConnectionTimeoutMilliseconds)
            && !TryInvoke(
                manager,
                "Connect",
                out loginResult,
                _options.Server,
                _options.ManagerLogin,
                _options.Password,
                0,
                _options.ConnectionTimeoutMilliseconds))
        {
            throw CreateMissingMethodException(manager, "Login", "Connect");
        }

        if (loginResult is bool isConnected && !isConnected)
        {
            throw new InvalidOperationException(
                $"MT5 manager login returned false for manager {_options.ManagerLogin} on {_options.Server}.");
        }

        _logger.LogInformation(
            "Connected to MT5 manager {ManagerName} ({ManagerLogin}) on {Server}.",
            _options.ManagerName,
            _options.ManagerLogin,
            _options.Server);
    }

    private static object? Invoke(object target, string methodName, params object[] arguments)
    {
        if (!TryInvoke(target, methodName, out var result, arguments))
        {
            throw CreateMissingMethodException(target, methodName);
        }

        return result;
    }

    private static bool TryInvoke(object target, string methodName, out object? result, params object[] arguments)
    {
        var method = FindMethod(target, methodName, arguments);
        if (method is null)
        {
            result = null;
            return false;
        }

        var convertedArguments = ConvertArguments(method.GetParameters(), arguments);
        result = method.Invoke(target, convertedArguments);
        return true;
    }

    private static MethodInfo? FindMethod(object target, string methodName, object[] arguments)
    {
        return target.GetType()
            .GetMethods(BindingFlags.Instance | BindingFlags.Public)
            .Where(candidate => string.Equals(candidate.Name, methodName, StringComparison.Ordinal))
            .FirstOrDefault(candidate => ParametersMatch(candidate.GetParameters(), arguments));
    }

    private static MissingMethodException CreateMissingMethodException(object target, params string[] methodNames)
    {
        var methodList = string.Join(" or ", methodNames);
        var availableMethods = string.Join(
            "; ",
            target.GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .Where(method => methodNames.Contains(method.Name, StringComparer.Ordinal))
                .Select(FormatMethodSignature));

        var message = string.IsNullOrWhiteSpace(availableMethods)
            ? $"Method {methodList} was not found on {target.GetType().FullName}."
            : $"No compatible {methodList} overload was found on {target.GetType().FullName}. Available overloads: {availableMethods}.";

        return new MissingMethodException(message);
    }

    private static string FormatMethodSignature(MethodInfo method)
    {
        var parameters = string.Join(
            ", ",
            method.GetParameters().Select(parameter => $"{parameter.ParameterType.Name} {parameter.Name}"));

        return $"{method.Name}({parameters})";
    }

    private static bool ParametersMatch(ParameterInfo[] parameters, object[] arguments)
    {
        if (parameters.Length != arguments.Length)
        {
            return false;
        }

        return parameters.Zip(arguments).All(pair => CanConvert(pair.Second, pair.First.ParameterType));
    }

    private static object?[] ConvertArguments(ParameterInfo[] parameters, object[] arguments)
    {
        return parameters.Zip(arguments)
            .Select(pair => ConvertArgument(pair.Second, pair.First.ParameterType))
            .ToArray();
    }

    private static bool CanConvert(object? value, Type targetType)
    {
        if (value is null)
        {
            return !targetType.IsValueType || Nullable.GetUnderlyingType(targetType) is not null;
        }

        var effectiveType = Nullable.GetUnderlyingType(targetType) ?? targetType;
        return effectiveType.IsInstanceOfType(value)
            || effectiveType.IsEnum
            || effectiveType == typeof(string)
            || typeof(IConvertible).IsAssignableFrom(effectiveType);
    }

    private static object? ConvertArgument(object? value, Type targetType)
    {
        if (value is null)
        {
            return null;
        }

        var effectiveType = Nullable.GetUnderlyingType(targetType) ?? targetType;
        if (effectiveType.IsInstanceOfType(value))
        {
            return value;
        }

        if (effectiveType.IsEnum)
        {
            return Enum.ToObject(effectiveType, value);
        }

        return Convert.ChangeType(value, effectiveType);
    }

    private static long ToManagerTimestamp(DateTime dateTime)
    {
        var utc = dateTime.Kind == DateTimeKind.Utc
            ? dateTime
            : DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);

        return new DateTimeOffset(utc).ToUnixTimeSeconds();
    }

    private sealed class ManagerInstance : IDisposable
    {
        public ManagerInstance(object instance)
        {
            Instance = instance;
        }

        public object Instance { get; }

        public void Dispose()
        {
            if (Instance is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}
