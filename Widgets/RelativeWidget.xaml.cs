using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using RaceLabsOverlay.Services;

namespace RaceLabsOverlay.Widgets
{
    public partial class RelativeWidget : UserControl
    {
        public string WidgetName => "Relative";
        public Size DefaultSize => new Size(220, 180);

        public RelativeWidget()
        {
            InitializeComponent();
        }

        public void Update(TelemetryData data)
        {
            Dispatcher.Invoke(() =>
            {
                Position1Text.Text = "P1";
                Car1Text.Text = "#24";
                Gap1Text.Text = "+0.234";
                Gap1Text.Foreground = new SolidColorBrush(Color.FromRgb(255, 100, 100));
                
                Position2Text.Text = "P2";
                Car2Text.Text = "YOU";
                Gap2Text.Text = "——";
                Gap2Text.Foreground = Brushes.LimeGreen;
                Background2.Opacity = 0.3;
                
                Position3Text.Text = "P3";
                Car3Text.Text = "#07";
                Gap3Text.Text = "-0.567";
                Gap3Text.Foreground = new SolidColorBrush(Color.FromRgb(100, 255, 100));
                
                TotalCarsText.Text = "/ 24 cars";
                LapsToLeaderText.Text = "+2 laps to leader";
            });
        }
    }
}
