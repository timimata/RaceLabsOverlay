using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using RaceLabsOverlay.Services;

namespace RaceLabsOverlay.Widgets
{
    public partial class TelemetryGraphWidget : UserControl
    {
        public string WidgetName => "Telemetry Graphs";
        public Size DefaultSize => new Size(500, 200);

        private readonly Queue<double> _speedData = new();
        private readonly Queue<double> _throttleData = new();
        private readonly Queue<double> _brakeData = new();
        private const int MaxPoints = 100;

        public TelemetryGraphWidget()
        {
            InitializeComponent();
        }

        public void Update(TelemetryData data)
        {
            _speedData.Enqueue(data.Speed);
            _throttleData.Enqueue(data.Throttle * 100);
            _brakeData.Enqueue(data.Brake * 100);

            if (_speedData.Count > MaxPoints)
            {
                _speedData.Dequeue();
                _throttleData.Dequeue();
                _brakeData.Dequeue();
            }

            Dispatcher.Invoke(() =>
            {
                CurrentSpeedText.Text = $"{data.Speed:F0} km/h";
                CurrentRpmText.Text = $"{data.RPM:F0} RPM";
                CurrentGearText.Text = $"Gear {data.Gear}";

                UpdatePolyline(SpeedLine, _speedData, 350);
                UpdatePolyline(ThrottleLine, _throttleData, 100);
                UpdatePolyline(BrakeLine, _brakeData, 100);
            });
        }

        private void UpdatePolyline(Polyline line, Queue<double> data, double maxValue)
        {
            var points = new PointCollection();
            double xStep = line.ActualWidth > 0 ? line.ActualWidth / MaxPoints : 4;
            double height = line.ActualHeight > 0 ? line.ActualHeight : 50;

            int i = 0;
            foreach (var value in data)
            {
                double x = i * xStep;
                double y = height - (value / maxValue * height);
                points.Add(new Point(x, Math.Clamp(y, 0, height)));
                i++;
            }

            line.Points = points;
        }
    }
}
