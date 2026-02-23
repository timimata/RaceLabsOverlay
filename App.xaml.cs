using System;
using System.Windows;
using RaceLabsOverlay.Services;
using RaceLabsOverlay.UI;

namespace RaceLabsOverlay
{
    public partial class App : Application
    {
        private UpdateService? _updateService;
        private SettingsService? _settingsService;

        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                base.OnStartup(e);

                // Initialize professional services
                LoggingService.Initialize();
                ErrorHandlingService.Initialize();

                LoggingService.LogInformation("=== RaceLabs Overlay Starting ===");

                // Load settings
                _settingsService = new SettingsService();
                LoggingService.LogInformation("Settings loaded");

                // OverlayWindow is created by StartupUri in App.xaml
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
            LoggingService.LogInformation($"=== RaceLabs Overlay Exiting (Code: {e.ApplicationExitCode}) ===");
            base.OnExit(e);
        }
    }
}
