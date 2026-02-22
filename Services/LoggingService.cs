using Serilog;
using Serilog.Events;
using System;
using System.IO;

namespace RaceLabsOverlay.Services
{
    /// <summary>
    /// Sistema profissional de logging.
    /// </summary>
    public static class LoggingService
    {
        private static ILogger? _logger;
        private static bool _initialized;

        public static void Initialize()
        {
            if (_initialized) return;

            var logPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "RaceLabsOverlay", "Logs");

            Directory.CreateDirectory(logPath);

            _logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
                .WriteTo.File(
                    path: Path.Combine(logPath, "log-.txt"),
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 7,
                    outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff} {Level:u3}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();

            _initialized = true;

            LogInformation("Logging initialized");
            var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version?.ToString(3) ?? "0.1.0";
            LogInformation($"Application version: {version}");
            LogInformation($"OS: {Environment.OSVersion}");
            LogInformation($"Machine: {Environment.MachineName}");
        }

        public static ILogger Logger => _logger ?? throw new InvalidOperationException("Logging not initialized");

        public static void LogDebug(string message) => _logger?.Debug(message);
        public static void LogInformation(string message) => _logger?.Information(message);
        public static void LogWarning(string message) => _logger?.Warning(message);
        public static void LogError(string message) => _logger?.Error(message);
        public static void LogError(Exception ex, string message) => _logger?.Error(ex, message);
        public static void LogFatal(string message) => _logger?.Fatal(message);
        public static void LogFatal(Exception ex, string message) => _logger?.Fatal(ex, message);
    }
}
