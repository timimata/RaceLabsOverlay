using System.Windows.Controls;
using System.Windows.Media;

namespace RaceLabsOverlay.Widgets
{
    public partial class SpeedometerWidget : UserControl
    {
        public SpeedometerWidget()
        {
            InitializeComponent();
        }

        public void UpdateSpeed(double speed)
        {
            SpeedText.Text = speed.ToString("0");
        }
    }
}