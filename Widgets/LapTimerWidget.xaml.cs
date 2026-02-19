using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RaceLabsOverlay.Widgets
{
    public partial class LapTimerWidget : UserControl, IWidget
    {
        public string WidgetName => "Lap Timer";
        public Size DefaultSize => new Size(220, 150);
        public bool IsConfigurable => false;

        public LapTimerWidget()
        {
            InitializeComponent();
        }

        public void Update(TelemetryData data)
        {
            Dispatcher.Invoke(() =>
            {
                LapNumberText.Text = data.Lap.ToString();
                CurrentLapText.Text = FormatTime(data.LapCurrentLapTime);
                LastLapText.Text = data.LapLastLapTime > 0 ? FormatTime(data.LapLastLapTime) : "—";
                BestLapText.Text = data.LapBestLapTime > 0 ? FormatTime(data.LapBestLapTime) : "—";
            });
        }

        private string FormatTime(float seconds)
        {
            var ts = TimeSpan.FromSeconds(seconds);
            return $"{ts.Minutes}:{ts.Seconds:00}.{ts.Milliseconds:000}";
        }

        public void Configure() { }
    }
}
