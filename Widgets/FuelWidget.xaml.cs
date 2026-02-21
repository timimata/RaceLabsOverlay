using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RaceLabsOverlay.Widgets
{
    public partial class FuelWidget : UserControl
    {
        public string WidgetName => "Fuel Info";
        public Size DefaultSize => new Size(200, 120);

        public FuelWidget()
        {
            InitializeComponent();
        }

        public void Update(TelemetryData data)
        {
            Dispatcher.Invoke(() =>
            {
                FuelText.Text = $"{data.FuelLevel:F1}";
                
                // Barra de percentagem (assumindo 60L max)
                double maxWidth = 180;
                double fuelPct = System.Math.Min(data.FuelLevel / 60.0 * 100, 100);
                FuelBar.Width = maxWidth * (fuelPct / 100);
                
                // Cor baseada no nível
                if (fuelPct < 10)
                    FuelBar.Fill = Brushes.Red;
                else if (fuelPct < 25)
                    FuelBar.Fill = Brushes.Orange;
                else
                    FuelBar.Fill = new SolidColorBrush(Color.FromRgb(0, 136, 255));
                
                LapsRemainingText.Text = $"{data.FuelLapsRemaining:F0}";
                ConsumptionText.Text = $"{data.FuelPerLap:F1}";
            });
        }
    }
}
