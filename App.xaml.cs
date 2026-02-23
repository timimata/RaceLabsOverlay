using System;
using System.Drawing;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using Hardcodet.Wpf.TaskbarNotification;
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
        private TaskbarIcon? _trayIcon;
        private WidgetPanelWindow? _widgetPanel;

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

                _overlayWindow = new OverlayWindow(_settingsService);

                // Widget panel (opened from tray or Ctrl+Shift+W)
                _widgetPanel = new WidgetPanelWindow(_overlayWindow.WidgetManager, _overlayWindow);
                _overlayWindow.OnOpenWidgetPanel += () => _widgetPanel.Show();

                // System tray icon
                SetupTrayIcon();

                // iRacing telemetry
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

                _telemetryProvider.OnTelemetryUpdated += (s, data) =>
                {
                    Dispatcher.Invoke(() => _overlayWindow.UpdateTelemetry(data));
                };

                _appCts = new CancellationTokenSource();
                _ = _telemetryProvider.StartAsync(_appCts.Token);
                LoggingService.LogInformation("Telemetry provider started, waiting for iRacing...");

                // Update service
                _updateService = new UpdateService(AppVersion.Current, "https://updates.racelabs.app");
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

        private void SetupTrayIcon()
        {
            _trayIcon = new TaskbarIcon
            {
                Icon = SystemIcons.Application,
                ToolTipText = "RaceLabs Overlay"
            };

            var contextMenu = new ContextMenu
            {
                Background = System.Windows.Media.Brushes.Black,
                Foreground = System.Windows.Media.Brushes.White
            };

            var widgetsItem = new MenuItem { Header = "Widgets" };
            widgetsItem.Click += (s, e) => _widgetPanel?.Show();
            contextMenu.Items.Add(widgetsItem);

            var editItem = new MenuItem { Header = "Edit Mode" };
            editItem.Click += (s, e) => _overlayWindow?.ToggleEditModePublic();
            contextMenu.Items.Add(editItem);

            contextMenu.Items.Add(new Separator());

            var exitItem = new MenuItem { Header = "Exit" };
            exitItem.Click += (s, e) => Shutdown();
            contextMenu.Items.Add(exitItem);

            _trayIcon.ContextMenu = contextMenu;
            _trayIcon.TrayMouseDoubleClick += (s, e) => _widgetPanel?.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            LoggingService.LogInformation("=== RaceLabs Overlay Shutting Down ===");

            _overlayWindow?.WidgetManager.SaveLayout();
            _appCts?.Cancel();
            _telemetryProvider?.Dispose();
            _trayIcon?.Dispose();

            LoggingService.LogInformation($"=== RaceLabs Overlay Exited (Code: {e.ApplicationExitCode}) ===");
            base.OnExit(e);
        }
    }
}
