using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using RaceLabsOverlay.Services;

namespace RaceLabsOverlay.UI
{
    public partial class UpdateDialog : Window
    {
        private readonly UpdateInfo _updateInfo;
        private readonly UpdateService _updateService;

        public UpdateDialog(UpdateInfo updateInfo, UpdateService updateService)
        {
            InitializeComponent();
            
            _updateInfo = updateInfo;
            _updateService = updateService;
            
            // Set version info
            CurrentVersionText.Text = $"Current: v{AppVersion.Current}";
            NewVersionText.Text = $"New: v{updateInfo.Version}";
            ReleaseNotesText.Text = updateInfo.ReleaseNotes ?? "No release notes available.";
            
            // Hide skip button if mandatory
            if (updateInfo.IsMandatory)
            {
                SkipButton.Visibility = Visibility.Collapsed;
                LaterButton.Visibility = Visibility.Collapsed;
            }
        }

        private async void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateButton.IsEnabled = false;
            LaterButton.IsEnabled = false;
            SkipButton.IsEnabled = false;
            
            ProgressGrid.Visibility = Visibility.Visible;
            
            try
            {
                var progress = new Progress<UpdateProgress>(p =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        DownloadProgress.Value = p.Percentage;
                        ProgressText.Text = p.Status;
                    });
                });
                
                await _updateService.DownloadAndInstallUpdateAsync(_updateInfo, progress);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Update failed: {ex.Message}",
                    "Update Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                
                UpdateButton.IsEnabled = true;
                LaterButton.IsEnabled = true;
                SkipButton.IsEnabled = true;
                ProgressGrid.Visibility = Visibility.Collapsed;
            }
        }

        private void LaterButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void SkipButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }

    public static class AppVersion
    {
        public static string Current => typeof(App).Assembly.GetName().Version?.ToString(3) ?? "0.1.0";
    }
}
