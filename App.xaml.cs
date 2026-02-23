using System;
using System.Threading;
using System.Windows;
using RaceLabsOverlay.Core.Telemetry;
using RaceLabsOverlay.Services;
using RaceLabsOverlay.UI;

namespace RaceLabsOverlay
{
    public partial class App : Application
    {
        private IRacingProvider? _telemetryProvider;
        private OverlayWindow? _overlayWindow;
        private UpdateService? _updateService;
        private SettingsService? _settingsService;
        private CancellationTokenSource? _appCts;

        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                base.OnStartup(e);

                LoggingService.Initialize();
                ErrorHandlingService.Initialize();
                LoggingService.LogInformation("=== RaceLabs Overlay Starting ===");

                _settingsService = new SettingsService();
                LoggingService.LogInformation("Settings loaded");

                // Create overlay window
                _overlayWindow = new OverlayWindow();

                // Initialize iRacing telemetry provider
                _telemetryProvider = new IRacingProvider();

                _telemetryProvider.OnConnected += (s, _) =>
                {
                    LoggingService.LogInformation("iRacing connected!");
                    Dispatcher.Invoke(() => _overlayWindow.ShowConnectionStatus(true));
                };

                _telemetryProvider.OnDisconnected += (s, _) =>
                {
                    LoggingService.LogInformation("iRacing disconnected, waiting for reconnect...");
                    Dispatcher.Invoke(() => _overlayWindow.ShowConnectionStatus(false));
                };

                // Wire real telemetry data to overlay widgets
                _telemetryProvider.OnTelemetryUpdated += (s, data) =>
                {
                    Dispatcher.Invoke(() => _overlayWindow.UpdateTelemetry(data));
                };

                // Start telemetry polling in background
                _appCts = new CancellationTokenSource();
                _ = _telemetryProvider.StartAsync(_appCts.Token);
                LoggingService.LogInformation("Telemetry provider started, waiting for iRacing...");

                // Initialize update service
                _updateService = new UpdateService(
                    AppVersion.Current,
                    "https://updates.racelabs.app");

                _updateService.OnUpdateAvailable += (s, args) =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        if (args.UpdateInfo.Version != _settingsService.Settings.SkippedVersion)
                        {
                            var dialog = new UpdateDialog(args.UpdateInfo, _updateService);
                            dialog.Show();
                        }
                    });
                };

                _updateService.OnUpdateError += (s, error) =>
                {
                    LoggingService.LogError($"Update error: {error}");
                };

                if (_settingsService.Settings.CheckForUpdatesOnStartup)
                {
                    _ = _updateService.CheckOnStartupAsync();
                }

                _overlayWindow.Show();
                LoggingService.LogInformation("Application started successfully");
            }
            catch (Exception ex)
            {
                LoggingService.LogFatal(ex, "Fatal error during startup");
                MessageBox.Show(
                    $"Failed to start application:\n\n{ex.Message}\n\nPlease check the logs.",
                    "Startup Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                Shutdown(1);
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            LoggingService.LogInformation("=== RaceLabs Overlay Shutting Down ===");

            _appCts?.Cancel();
            _telemetryProvider?.Dispose();

            LoggingService.LogInformation($"=== RaceLabs Overlay Exited (Code: {e.ApplicationExitCode}) ===");
            base.OnExit(e);
        }
    }
}
