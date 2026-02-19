using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RaceLabsOverlay.Widgets
{
    /// <summary>
    /// Widget de Standings - posição na corrida.
    /// </summary>
    public partial class StandingsWidget : UserControl, IWidget
    {
        public string WidgetName => "Standings";
        public Size DefaultSize => new Size(180, 120);
        public bool IsConfigurable => false;

        public StandingsWidget()
        {
            InitializeComponent();
        }

        public void Update(TelemetryData data)
        {
            Dispatcher.Invoke(() =>
            {
                // Posição atual
                PositionBig.Text = "2";
                PositionSuffix.Text = "nd";
                TotalCars.Text = "/ 24";
                
                // Gap info
                GapToLeader.Text = "+2.456";
                GapToLeader.Foreground = Brushes.Red;
                
                GapToNext.Text = "+0.234";
                GapToNext.Foreground = Brushes.LimeGreen;
                
                // Laps
                CurrentLap.Text = "Lap 12/50";
                
                // Class position (se aplicável)
                ClassPosition.Text = "P2 in Class";
            });
        }

        public void Configure() { }
    }
}
