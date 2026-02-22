using System.Windows;
using RaceLabsOverlay.Services;

namespace RaceLabsOverlay.UI
{
    public partial class SettingsWindow : Window
    {
        private readonly SettingsService _settingsService;

        public SettingsWindow(SettingsService settingsService)
        {
            InitializeComponent();
            _settingsService = settingsService;

            // Load current settings into UI
            var s = _settingsService.Settings;
            StartWithWindowsCheck.IsChecked = s.StartWithWindows;
            CheckUpdatesCheck.IsChecked = s.CheckForUpdatesOnStartup;
            MinimizeToTrayCheck.IsChecked = s.MinimizeToTray;
            OpacitySlider.Value = s.OverlayOpacity;
            OpacityValue.Text = $"{(int)(s.OverlayOpacity * 100)}%";
            ClickThroughCheck.IsChecked = s.ClickThroughByDefault;
            EnableGhostCheck.IsChecked = s.EnableGhostRecording;
            AutoSaveGhostCheck.IsChecked = s.AutoSaveGhostOnBestLap;
            EditModeHotkey.Text = s.ToggleEditModeHotkey;
            ClickThroughHotkey.Text = s.ToggleClickThroughHotkey;
            RecordGhostHotkey.Text = s.RecordGhostHotkey;
            VersionText.Text = $"Version {AppVersion.Current}";

            OpacitySlider.ValueChanged += (_, e) =>
                OpacityValue.Text = $"{(int)(e.NewValue * 100)}%";
        }

        private void CheckUpdateButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Update check is handled on startup.", "Check for Updates",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var s = _settingsService.Settings;
            s.StartWithWindows = StartWithWindowsCheck.IsChecked == true;
            s.CheckForUpdatesOnStartup = CheckUpdatesCheck.IsChecked == true;
            s.MinimizeToTray = MinimizeToTrayCheck.IsChecked == true;
            s.OverlayOpacity = OpacitySlider.Value;
            s.ClickThroughByDefault = ClickThroughCheck.IsChecked == true;
            s.EnableGhostRecording = EnableGhostCheck.IsChecked == true;
            s.AutoSaveGhostOnBestLap = AutoSaveGhostCheck.IsChecked == true;
            s.ToggleEditModeHotkey = EditModeHotkey.Text;
            s.ToggleClickThroughHotkey = ClickThroughHotkey.Text;
            s.RecordGhostHotkey = RecordGhostHotkey.Text;
            _settingsService.Save();
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
