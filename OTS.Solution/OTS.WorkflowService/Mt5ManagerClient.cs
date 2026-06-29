using System.Reflection;
using System.Text.Json;

namespace OTS.WorkflowService;

public sealed class Mt5ManagerClient : IDisposable
{
    private readonly Mt5ManagerOptions _options;
    private readonly ILogger<Mt5ManagerClient> _logger;
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
        InitializeFactory(assembly);
        _manager = CreateManager(assembly);
        InvokeByName(_manager, "Connect", _options.Server, _options.Login, _options.Password, null, _options.PumpingMode, _options.TimeoutMilliseconds);
        _connected = true;
        _logger.LogInformation("Connected to MT5 Manager server {Server} as login {Login}.", _options.Server, _options.Login);
    }

    public Mt5Snapshot GetTradeAndOrderSnapshot()
    {
        Connect();
        ArgumentNullException.ThrowIfNull(_manager);

        var orders = InvokeFirstAvailable(_manager,
            new[] { "OrderGet", "OrdersGet", "OrderGetAll", "OrderGetPage", "OrdersGetPage" },
            new object?[][]
            {
                Array.Empty<object?>(),
                new object?[] { _options.PreviewCount },
                new object?[] { 0u, _options.PreviewCount },
                new object?[] { 0UL, _options.PreviewCount }
            });

        var trades = InvokeFirstAvailable(_manager,
            new[] { "DealGet", "DealsGet", "DealGetAll", "TradeGet", "TradesGet", "PositionGet", "PositionsGet" },
            new object?[][]
            {
                Array.Empty<object?>(),
                new object?[] { _options.PreviewCount },
                new object?[] { 0u, _options.PreviewCount },
                new object?[] { 0UL, _options.PreviewCount }
            });

        return new Mt5Snapshot(Normalize(orders), Normalize(trades));
    }

    public void Dispose()
    {
        if (_manager is null)
        {
            return;
        }

        InvokeOptional(_manager, "Disconnect");
        (_manager as IDisposable)?.Dispose();
        _connected = false;
    }

    private static Assembly LoadManagerAssembly()
    {
        return AppDomain.CurrentDomain.GetAssemblies()
                   .FirstOrDefault(a => a.GetName().Name?.Contains("MT5ManagerAPI", StringComparison.OrdinalIgnoreCase) == true)
               ?? Assembly.Load("MetaQuotes.MT5ManagerAPI64-net2.0");
    }

    private static void InitializeFactory(Assembly assembly)
    {
        var factory = assembly.GetTypes().FirstOrDefault(t => t.Name.Contains("Factory", StringComparison.OrdinalIgnoreCase));
        if (factory is null)
        {
            return;
        }

        InvokeOptional(factory, "Initialize");
        InvokeOptional(factory, "Init");
    }

    private static object CreateManager(Assembly assembly)
    {
        var factory = assembly.GetTypes().FirstOrDefault(t => t.Name.Contains("Factory", StringComparison.OrdinalIgnoreCase));
        if (factory is not null)
        {
            foreach (var methodName in new[] { "CreateManager", "Create", "ManagerCreate" })
            {
                var manager = InvokeOptional(factory, methodName);
                if (manager is not null)
                {
                    return manager;
                }
            }
        }

        var managerType = assembly.GetTypes().FirstOrDefault(t => t.Name.Contains("Manager", StringComparison.OrdinalIgnoreCase) && !t.IsInterface && t.GetConstructor(Type.EmptyTypes) is not null)
            ?? throw new InvalidOperationException("Unable to find an MT5 manager type in the MetaQuotes assembly.");

        return Activator.CreateInstance(managerType)!;
    }

    private static object? InvokeFirstAvailable(object target, IEnumerable<string> methodNames, IEnumerable<object?[]> argumentSets)
    {
        foreach (var methodName in methodNames)
        {
            foreach (var arguments in argumentSets)
            {
                var value = InvokeOptional(target, methodName, arguments);
                if (value is not null)
                {
                    return value;
                }
            }
        }

        throw new MissingMethodException("No compatible MT5 manager method was found for reading orders/trades.");
    }

    private static object? InvokeOptional(object target, string methodName, params object?[] arguments)
    {
        try
        {
            return InvokeByName(target, methodName, arguments);
        }
        catch (MissingMethodException)
        {
            return null;
        }
    }

    private static object? InvokeByName(object target, string methodName, params object?[] arguments)
    {
        var type = target as Type ?? target.GetType();
        var flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;
        var method = type.GetMethods(flags)
            .FirstOrDefault(m => string.Equals(m.Name, methodName, StringComparison.OrdinalIgnoreCase) && m.GetParameters().Length == arguments.Length);

        if (method is null)
        {
            throw new MissingMethodException(type.FullName, methodName);
        }

        return method.Invoke(target is Type ? null : target, arguments);
    }

    private static IReadOnlyList<string> Normalize(object? value)
    {
        if (value is null)
        {
            return Array.Empty<string>();
        }

        if (value is System.Collections.IEnumerable enumerable && value is not string)
        {
            return enumerable.Cast<object?>().Select(Serialize).ToList();
        }

        return new[] { Serialize(value) };
    }

    private static string Serialize(object? value)
    {
        return JsonSerializer.Serialize(value, value?.GetType() ?? typeof(object), new JsonSerializerOptions { WriteIndented = false });
    }
}

public sealed record Mt5Snapshot(IReadOnlyList<string> Orders, IReadOnlyList<string> Trades);
