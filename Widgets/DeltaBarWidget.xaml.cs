using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using RaceLabsOverlay.Services;

namespace RaceLabsOverlay.Widgets
{
    public partial class DeltaBarWidget : UserControl
    {
        public string WidgetName => "Delta Bar";
        public Size DefaultSize => new Size(300, 60);

        public DeltaBarWidget()
        {
            InitializeComponent();
            UpdateDisplay(0, false);
        }

        public void Update(TelemetryData data)
        {
            UpdateDisplay(data.LapCurrentLapTime - data.LapBestLapTime, true);
        }

        private void UpdateDisplay(float delta, bool isValid)
        {
            Dispatcher.Invoke(() =>
            {
                if (!isValid || delta == 0)
                {
                    DeltaText.Text = "—.——";
                    DeltaText.Foreground = Brushes.Gray;
                    PositiveBar.Width = 0;
                    NegativeBar.Width = 0;
                    return;
                }
                
                string sign = delta >= 0 ? "+" : "";
                DeltaText.Text = $"{sign}{delta:F2}";
                
                if (delta < 0)
                {
                    DeltaText.Foreground = Brushes.LimeGreen;
                    NegativeBar.Fill = Brushes.LimeGreen;
                    NegativeBar.Width = Math.Min(Math.Abs(delta) * 50, 100);
                    PositiveBar.Width = 0;
                }
                else
                {
                    DeltaText.Foreground = delta > 1.0 ? Brushes.Red : Brushes.Orange;
                    PositiveBar.Fill = delta > 1.0 ? Brushes.Red : Brushes.Orange;
                    PositiveBar.Width = Math.Min(delta * 50, 100);
                    NegativeBar.Width = 0;
                }
            });
        }
    }
}
