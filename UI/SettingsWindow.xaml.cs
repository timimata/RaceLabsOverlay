using System;
using System.Windows;
using System.Windows.Controls;
using RaceLabsOverlay.Services;

namespace RaceLabsOverlay.UI
{
    public partial class SettingsWindow : Window
    {
        private readonly SettingsService _settingsService;

        public SettingsWindow(SettingsService settingsService)
        {
            _settingsService = settingsService;
            InitializeComponent();
            LoadSettings();
        }

        private void LoadSettings()
        {
            // Load settings into UI
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Save settings
            _settingsService.Save();
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}