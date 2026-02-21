using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using RaceLabsOverlay.Services;

namespace RaceLabsOverlay.Widgets
{
    public partial class GhostComparatorWidget : UserControl
    {
        public string WidgetName => "Ghost Comparator";
        public Size DefaultSize => new Size(400, 100);

        public GhostComparatorWidget()
        {
            InitializeComponent();
        }

        public void Update(TelemetryData data)
        {
            Dispatcher.Invoke(() =>
            {
                float delta = data.LapCurrentLapTime - data.LapBestLapTime;
                string sign = delta >= 0 ? "+" : "";
                DeltaText.Text = $"{sign}{delta:F2}";
                
                if (delta < 0)
                {
                    DeltaText.Foreground = Brushes.LimeGreen;
                    StatusText.Text = "AHEAD";
                    StatusText.Foreground = Brushes.LimeGreen;
                    AheadBar.Width = Math.Min(Math.Abs(delta) * 50, 150);
                    BehindBar.Width = 0;
                }
                else if (delta > 0)
                {
                    DeltaText.Foreground = Brushes.Red;
                    StatusText.Text = "BEHIND";
                    StatusText.Foreground = Brushes.Red;
                    BehindBar.Width = Math.Min(delta * 50, 150);
                    AheadBar.Width = 0;
                }
                else
                {
                    DeltaText.Foreground = Brushes.White;
                    StatusText.Text = "ON TRACK";
                    StatusText.Foreground = Brushes.Gray;
                    AheadBar.Width = 0;
                    BehindBar.Width = 0;
                }
                
                GhostSpeedText.Text = $"Ghost: {data.Speed:F0} km/h";
            });
        }
    }
}
