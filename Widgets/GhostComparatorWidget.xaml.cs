using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RaceLabsOverlay.Widgets
{
    public partial class GhostComparatorWidget : UserControl, IWidget
    {
        public string WidgetName => "Ghost Comparator";
        public Size DefaultSize => new Size(400, 100);
        public bool IsConfigurable => false;

        private readonly Brush _aheadColor = Brushes.LimeGreen;
        private readonly Brush _behindColor = Brushes.Red;

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
                    DeltaText.Foreground = _aheadColor;
                    StatusText.Text = "AHEAD";
                    StatusText.Foreground = _aheadColor;
                    UpdateComparisonBar(delta);
                }
                else if (delta > 0)
                {
                    DeltaText.Foreground = _behindColor;
                    StatusText.Text = "BEHIND";
                    StatusText.Foreground = _behindColor;
                    UpdateComparisonBar(delta);
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

                // Update track position markers
                UpdateTrackPosition(data.LapDistPct);
            });
        }

        private void UpdateComparisonBar(float delta)
        {
            double maxDelta = 2.0;
            double percentage = Math.Min(Math.Abs(delta) / maxDelta, 1.0);
            double barWidth = BarContainer.ActualWidth * percentage / 2;

            if (delta < 0)
            {
                AheadBar.Width = barWidth;
                BehindBar.Width = 0;
            }
            else
            {
                AheadBar.Width = 0;
                BehindBar.Width = barWidth;
            }

            ArrowIndicator.RenderTransform = new TranslateTransform(
                delta < 0 ? -20 : 20, 0);
        }

        private void UpdateTrackPosition(float lapDistPct)
        {
            double trackWidth = TrackRepresentation.ActualWidth;
            double playerPos = lapDistPct * trackWidth;
            PlayerMarker.RenderTransform = new TranslateTransform(playerPos, 0);
            GhostMarker.RenderTransform = new TranslateTransform(playerPos * 0.95, 0);
        }

        public void Configure() { }
    }
}
