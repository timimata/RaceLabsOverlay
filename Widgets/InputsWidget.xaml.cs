using System.Windows;
using System.Windows.Controls;
using RaceLabsOverlay.Services;

namespace RaceLabsOverlay.Widgets
{
    public partial class InputsWidget : UserControl
    {
        public string WidgetName => "Inputs";
        public Size DefaultSize => new Size(150, 200);

        public InputsWidget()
        {
            InitializeComponent();
        }

        public void Update(TelemetryData data)
        {
            Dispatcher.Invoke(() =>
            {
                // Values are 0-1, convert to 0-100%
                ThrottleBar.Width = 95 * data.Throttle;
                BrakeBar.Width = 95 * data.Brake;
                ClutchBar.Width = 95 * data.Clutch;
            });
        }
    }
}
