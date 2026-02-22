using System.Windows;
using System.Windows.Controls;

namespace RaceLabsOverlay.Widgets
{
    public partial class InputsWidget : UserControl, IWidget
    {
        public string WidgetName => "Inputs";
        public Size DefaultSize => new Size(150, 200);
        public bool IsConfigurable => false;

        public InputsWidget()
        {
            InitializeComponent();
        }

        public void Update(TelemetryData data)
        {
            Dispatcher.Invoke(() =>
            {
                ThrottleBar.Width = (ThrottleBar.Parent as Border)?.ActualWidth * (data.Throttle / 100) ?? 0;
                BrakeBar.Width = (BrakeBar.Parent as Border)?.ActualWidth * (data.Brake / 100) ?? 0;
                ClutchBar.Width = (ClutchBar.Parent as Border)?.ActualWidth * (data.Clutch / 100) ?? 0;
            });
        }

        public void Configure() { }
    }
}
