using System.Diagnostics;
using Microsoft.Extensions.Options;
using OTS.WorkflowService.Options;

namespace OTS.WorkflowService.Services
{
    /// <summary>
    /// Optionally starts the OTS.Mt5Bridge sidecar before the worker begins polling.
    /// The sidecar owns the native MT5 Manager API connection and receives the
    /// manager credentials through environment variables.
    /// </summary>
    public sealed class Mt5BridgeProcessHostedService : IHostedService, IDisposable
    {
        private readonly ILogger<Mt5BridgeProcessHostedService> _logger;
        private readonly Mt5SyncOptions _options;
        private Process? _process;

        public Mt5BridgeProcessHostedService(
            ILogger<Mt5BridgeProcessHostedService> logger,
            IOptions<Mt5SyncOptions> options)
        {
            _logger = logger;
            _options = options.Value;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            if (!_options.AutoStartBridge)
            {
                _logger.LogInformation("MT5 bridge auto-start is disabled; expecting bridge at {BridgeBaseUrl}", _options.BridgeBaseUrl);
                return Task.CompletedTask;
            }

            if (string.IsNullOrWhiteSpace(_options.BridgeExecutablePath))
            {
                _logger.LogWarning("MT5 bridge auto-start is enabled but BridgeExecutablePath is empty; expecting bridge at {BridgeBaseUrl}", _options.BridgeBaseUrl);
                return Task.CompletedTask;
            }

            var executablePath = Environment.ExpandEnvironmentVariables(_options.BridgeExecutablePath);
            if (!File.Exists(executablePath))
            {
                _logger.LogWarning("MT5 bridge executable was not found at {BridgeExecutablePath}; expecting bridge at {BridgeBaseUrl}", executablePath, _options.BridgeBaseUrl);
                return Task.CompletedTask;
            }

            var startInfo = new ProcessStartInfo
            {
                FileName = executablePath,
                WorkingDirectory = Path.GetDirectoryName(executablePath) ?? AppContext.BaseDirectory,
                UseShellExecute = false
            };

            startInfo.Environment["MT5_SERVER"] = _options.Server;
            startInfo.Environment["MT5_LOGIN"] = _options.Login.ToString();
            startInfo.Environment["MT5_PASSWORD"] = _options.Password;
            startInfo.Environment["MT5_BRIDGE_URL"] = _options.BridgeBaseUrl.TrimEnd('/');

            _process = Process.Start(startInfo);
            if (_process is null)
            {
                _logger.LogWarning("Failed to start MT5 bridge process from {BridgeExecutablePath}", executablePath);
                return Task.CompletedTask;
            }

            _logger.LogInformation(
                "Started MT5 bridge process {ProcessId} for server {Server} login {Login} ({ManagerName}) at {BridgeBaseUrl}",
                _process.Id,
                _options.Server,
                _options.Login,
                _options.ManagerName,
                _options.BridgeBaseUrl);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            if (_process is { HasExited: false })
            {
                _logger.LogInformation("Stopping MT5 bridge process {ProcessId}", _process.Id);
                _process.Kill(entireProcessTree: true);
            }

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _process?.Dispose();
        }
    }
}
