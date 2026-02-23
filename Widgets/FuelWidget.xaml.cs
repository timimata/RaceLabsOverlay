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

                // Fuel bar (FuelLevelPct is 0-1 from iRacing)
                double maxWidth = (FuelBar.Parent as Border)?.ActualWidth ?? 100;
                double pct = data.FuelLevelPct * 100;
                FuelBar.Width = maxWidth * data.FuelLevelPct;

                // Color based on level
                if (pct < 10)
                    FuelBar.Fill = Brushes.Red;
                else if (pct < 25)
                    FuelBar.Fill = Brushes.Orange;
                else
                    FuelBar.Fill = new SolidColorBrush(Color.FromRgb(0, 136, 255));

                // Laps remaining (calculated by IRacingProvider)
                if (data.FuelLapsRemaining > 0)
                    LapsRemainingText.Text = $"{data.FuelLapsRemaining:F1}";
                else
                    LapsRemainingText.Text = "---";

                // Fuel per lap
                if (data.FuelPerLap > 0)
                    ConsumptionText.Text = $"{data.FuelPerLap:F2}";
                else
                    ConsumptionText.Text = "---";
            });
        }

        public void Configure() { }
    }
}
