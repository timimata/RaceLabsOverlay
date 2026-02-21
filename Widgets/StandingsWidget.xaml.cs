using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using RaceLabsOverlay.Services;

namespace RaceLabsOverlay.Widgets
{
    public partial class StandingsWidget : UserControl
    {
        public string WidgetName => "Standings";
        public Size DefaultSize => new Size(180, 120);

        public StandingsWidget()
        {
            InitializeComponent();
        }

        public void Update(TelemetryData data)
        {
            Dispatcher.Invoke(() =>
            {
                PositionBig.Text = "2";
                PositionSuffix.Text = "nd";
                TotalCars.Text = "/ 24";
                
                GapToLeader.Text = "+2.456";
                GapToLeader.Foreground = Brushes.Red;
                
                GapToNext.Text = "+0.234";
                GapToNext.Foreground = Brushes.LimeGreen;
                
                CurrentLap.Text = $"Lap {data.Lap}/50";
                ClassPosition.Text = "P2 in Class";
            });
        }
    }
}
