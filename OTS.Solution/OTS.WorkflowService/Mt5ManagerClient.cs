using System.Reflection;
using System.Text.Json;

namespace OTS.WorkflowService;

public sealed class Mt5ManagerClient
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = false };

    private readonly Mt5ManagerOptions _options;
    private readonly ILogger<Mt5ManagerClient> _logger;

    public Mt5ManagerClient(Mt5ManagerOptions options, ILogger<Mt5ManagerClient> logger)
    {
        _options = options;
        _logger = logger;
    }

    public Mt5Snapshot GetTradeAndOrderSnapshot()
    {
        var assembly = AppDomain.CurrentDomain.GetAssemblies()
            .FirstOrDefault(a => a.GetName().Name?.Contains("MetaTrader5", StringComparison.OrdinalIgnoreCase) == true)
            ?? TryLoadAssembly("MetaTrader5.Net.API")
            ?? TryLoadAssembly("MetaTrader5.Net");

        if (assembly is null)
        {
            throw new InvalidOperationException("MetaTrader5.Net.API assembly is not available. Ensure the MetaTrader5.Net.API package is restored and copied to the output folder.");
        }

        foreach (var apiType in assembly.GetTypes().Where(t => !t.IsAbstract && !t.IsInterface && t.GetConstructor(Type.EmptyTypes) is not null))
        {
            var api = Activator.CreateInstance(apiType);
            if (api is null || !TryConnect(api))
            {
                continue;
            }

            var from = DateTime.UtcNow.AddDays(-Math.Max(1, _options.DealHistoryDays));
            var to = DateTime.UtcNow;
            var orders = InvokeFirstDataMethod(api,
                new[] { "OrdersGet", "Orders", "GetOrders", "PositionsGet", "Positions", "GetPositions" },
                Array.Empty<object?>(),
                new object?[] { _options.OrderGroupMask });
            var deals = InvokeFirstDataMethod(api,
                new[] { "HistoryDealsGet", "DealsGet", "GetDeals", "HistoryOrdersGet", "GetHistoryOrders" },
                new object?[] { from, to },
                new object?[] { from, to, _options.OrderGroupMask });

            _logger.LogInformation("MT5 snapshot loaded with MetaTrader5.Net API type {ApiType}.", apiType.FullName);
            return new Mt5Snapshot(
                NormalizeRows(orders).Take((int)_options.MaxRows).ToList(),
                NormalizeRows(deals).Take((int)_options.MaxRows).ToList());
        }

        throw new InvalidOperationException("Unable to connect using MetaTrader5.Net.API with the configured MT5 credentials.");
    }

    private bool TryConnect(object api)
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
                    return method.Invoke(target, ConvertArguments(method.GetParameters(), arguments));
                }
                catch
                {
                    // Try the next overload/name.
                }
            }
        }

        return null;
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
