using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RaceLabsOverlay.Widgets
{
    public partial class SpeedometerWidget : UserControl, IWidget
    {
        public string WidgetName => "Speedometer";
        public Size DefaultSize => new Size(200, 120);
        public bool IsConfigurable => false;

        public SpeedometerWidget()
        {
            InitializeComponent();
        }

        public void Update(TelemetryData data)
        {
            Dispatcher.Invoke(() =>
            {
                SpeedText.Text = $"{data.Speed:F0}";
                
                // Cor baseada na velocidade
                if (data.Speed > 200)
                    SpeedText.Foreground = Brushes.Red;
                else if (data.Speed > 150)
                    SpeedText.Foreground = Brushes.Orange;
                else
                    SpeedText.Foreground = Brushes.White;
            });
        }

        public void Configure() { }
    }
}
