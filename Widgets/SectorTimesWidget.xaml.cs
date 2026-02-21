using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using RaceLabsOverlay.Services;

namespace RaceLabsOverlay.Widgets
{
    public partial class SectorTimesWidget : UserControl
    {
        public string WidgetName => "Sector Times";
        public Size DefaultSize => new Size(200, 140);

        public SectorTimesWidget()
        {
            InitializeComponent();
        }

        public void Update(TelemetryData data)
        {
            Dispatcher.Invoke(() =>
            {
                S1Time.Text = "32.456";
                S1Delta.Text = "-0.234";
                S1Delta.Foreground = Brushes.LimeGreen;
                S1Bar.Fill = Brushes.LimeGreen;
                S1Bar.Width = 45;
                
                S2Time.Text = "28.123";
                S2Delta.Text = "+0.012";
                S2Delta.Foreground = Brushes.Orange;
                S2Bar.Fill = Brushes.Orange;
                S2Bar.Width = 42;
                
                S3Time.Text = "24.890";
                S3Delta.Text = "-0.123";
                S3Delta.Foreground = Brushes.LimeGreen;
                S3Bar.Fill = Brushes.LimeGreen;
                S3Bar.Width = 48;
                
                PersonalBestTime.Text = "1:25.469";
                PersonalBestDelta.Text = "-0.345";
                PersonalBestDelta.Foreground = Brushes.Purple;
            });
        }
    }
}
