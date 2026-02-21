using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using RaceLabsOverlay.Services;

namespace RaceLabsOverlay.Widgets
{
    public partial class TrackMapWidget : UserControl
    {
        public string WidgetName => "Track Map";
        public Size DefaultSize => new Size(200, 200);
        
        private Point[] _trackPoints = Array.Empty<Point>();

        public TrackMapWidget()
        {
            InitializeComponent();
            GenerateSampleTrack();
        }

        private void GenerateSampleTrack()
        {
            var points = new System.Collections.Generic.List<Point>();
            int segments = 50;
            double centerX = 90;
            double centerY = 80;
            double radiusX = 70;
            double radiusY = 50;

            for (int i = 0; i <= segments; i++)
            {
                double angle = 2 * Math.PI * i / segments;
                double x = centerX + radiusX * Math.Cos(angle);
                double y = centerY + radiusY * Math.Sin(angle) * 0.6;
                
                if (i > segments / 4 && i < segments / 2)
                    x += 20;
                
                points.Add(new Point(x, y));
            }

            _trackPoints = points.ToArray();
            DrawTrack();
        }

        private void DrawTrack()
        {
            if (_trackPoints.Length == 0) return;

            var pathFigure = new PathFigure
            {
                StartPoint = _trackPoints[0],
                IsClosed = true
            };

            for (int i = 1; i < _trackPoints.Length; i++)
            {
                pathFigure.Segments.Add(new LineSegment(_trackPoints[i], true));
            }

            var pathGeometry = new PathGeometry();
            pathGeometry.Figures.Add(pathFigure);

            TrackPath.Data = pathGeometry;
        }

        public void Update(TelemetryData data)
        {
            Dispatcher.Invoke(() =>
            {
                if (_trackPoints.Length > 0)
                {
                    int index = (int)(data.LapDistPct * (_trackPoints.Length - 1));
                    index = Math.Clamp(index, 0, _trackPoints.Length - 1);
                    
                    var pos = _trackPoints[index];
                    Canvas.SetLeft(PlayerMarker, pos.X - 5);
                    Canvas.SetTop(PlayerMarker, pos.Y - 5);
                }
            });
        }
    }
}
