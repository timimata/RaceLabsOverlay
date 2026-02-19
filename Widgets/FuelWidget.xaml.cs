using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RaceLabsOverlay.Widgets
{
    public partial class FuelWidget : UserControl, IWidget
    {
        public string WidgetName => "Fuel Info";
        public Size DefaultSize => new Size(200, 120);
        public bool IsConfigurable => false;

        public FuelWidget()
        {
            InitializeComponent();
        }

        public void Update(TelemetryData data)
        {
            Dispatcher.Invoke(() =>
            {
                FuelText.Text = $"{data.FuelLevel:F1}";
                
                // Barra de percentagem
                double maxWidth = (FuelBar.Parent as Border)?.ActualWidth ?? 100;
                FuelBar.Width = maxWidth * (data.FuelLevelPct / 100);
                
                // Cor baseada no nível
                if (data.FuelLevelPct < 10)
                    FuelBar.Fill = Brushes.Red;
                else if (data.FuelLevelPct < 25)
                    FuelBar.Fill = Brushes.Orange;
                else
                    FuelBar.Fill = new SolidColorBrush(Color.FromRgb(0, 136, 255));
                
                // Calcular voltas restantes
                if (data.FuelUsePerHour > 0)
                {
                    // Assumindo volta de ~90s
                    float lapsRemaining = data.FuelLevel / (data.FuelUsePerHour / 3600 * 90);
                    LapsRemainingText.Text = $"{lapsRemaining:F0}";
                }
                else
                {
                    LapsRemainingText.Text = "—";
                }
                
                ConsumptionText.Text = $"{data.FuelUsePerHour:F1}";
            });
        }

        public void Configure() { }
    }
}
