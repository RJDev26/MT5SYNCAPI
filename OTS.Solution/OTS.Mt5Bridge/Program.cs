using System;
using System.Configuration;
using Microsoft.Owin.Hosting;
using OTS.Mt5Bridge.Api;
using OTS.Mt5Bridge.Mt5;

namespace OTS.Mt5Bridge
{
    /// <summary>
    /// Console entry point for the net48 MT5 bridge. Reads connection settings
    /// from app.config / environment, connects the Manager API, and self-hosts
    /// the HTTP API the .NET 8 worker calls.
    ///
    /// Run as a console app for dev, or wrap as a Windows Service for prod.
    /// </summary>
    internal static class Program
    {
        private static void Main()
        {
            var config = new Mt5BridgeConfig
            {
                Server = Env("MT5_SERVER", "Mt5:Server"),
                Login = ulong.Parse(Env("MT5_LOGIN", "Mt5:Login", "0")),
                Password = Env("MT5_PASSWORD", "Mt5:Password"),
                ListenUrl = Env("MT5_BRIDGE_URL", "Mt5:ListenUrl", "http://127.0.0.1:5099")
            };

            var client = new Mt5ManagerClient(config);
            Mt5Controller.Client = client;

            Console.WriteLine($"Connecting to MT5 {config.Server} as {config.Login} ...");
            try
            {
                client.EnsureConnected();
                Console.WriteLine("Connected.");
            }
            catch (Exception ex)
            {
                // Start anyway; /health reports disconnected and calls retry connect.
                Console.WriteLine("Initial connect failed: " + ex.Message);
            }

            using (WebApp.Start<Startup>(config.ListenUrl))
            {
                Console.WriteLine($"MT5 bridge listening on {config.ListenUrl}");
                Console.WriteLine("Press Ctrl+C to stop.");
                System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);
            }
        }

        private static string Env(string envVar, string appSettingKey, string fallback = "")
        {
            var v = Environment.GetEnvironmentVariable(envVar);
            if (!string.IsNullOrEmpty(v)) return v;
            v = ConfigurationManager.AppSettings[appSettingKey];
            return string.IsNullOrEmpty(v) ? fallback : v;
        }
    }
}
