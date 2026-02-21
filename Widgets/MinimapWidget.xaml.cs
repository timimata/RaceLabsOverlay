using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using RaceLabsOverlay.Services;

namespace RaceLabsOverlay.Widgets
{
    public partial class MinimapWidget : UserControl
    {
        public string WidgetName => "Minimap";
        public Size DefaultSize => new Size(200, 200);

        public MinimapWidget()
        {
            InitializeComponent();
            DrawTrack();
        }

        private void DrawTrack()
        {
            var trackPath = new Path
            {
                Stroke = new SolidColorBrush(Color.FromRgb(60, 60, 60)),
                StrokeThickness = 8,
                StrokeLineJoin = PenLineJoin.Round,
                Data = Geometry.Parse("M 30,100 Q 40,40 80,35 Q 120,30 140,60 Q 160,90 150,120 Q 140,150 100,160 Q 60,170 40,140 Q 20,110 30,100")
            };
            
            TrackCanvas.Children.Add(trackPath);
            
            AddCar(80, 35, Brushes.Red, true);
            AddCar(100, 100, Brushes.Orange, false);
            AddCar(50, 150, Brushes.Yellow, false);
        }

        private void AddCar(double x, double y, Brush color, bool isPlayer)
        {
            var car = new Ellipse
            {
                Width = isPlayer ? 8 : 6,
                Height = isPlayer ? 8 : 6,
                Fill = color,
                Stroke = isPlayer ? Brushes.White : null,
                StrokeThickness = isPlayer ? 2 : 0
            };
            
            Canvas.SetLeft(car, x - (isPlayer ? 4 : 3));
            Canvas.SetTop(car, y - (isPlayer ? 4 : 3));
            
            TrackCanvas.Children.Add(car);
        }

        public void Update(TelemetryData data)
        {
            Dispatcher.Invoke(() =>
            {
                // TODO: Implementar atualização real baseada em posição na pista
            });
        }
    }
}
