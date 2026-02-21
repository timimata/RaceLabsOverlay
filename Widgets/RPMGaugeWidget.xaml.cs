using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using RaceLabsOverlay.Services;

namespace RaceLabsOverlay.Widgets
{
    public partial class RPMGaugeWidget : UserControl
    {
        public string WidgetName => "RPM Gauge";
        public Size DefaultSize => new Size(200, 200);
        
        private float _maxRpm = 8000;
        private float _shiftLightRpm = 7500;
        private float _redlineRpm = 7800;
        
        public RPMGaugeWidget()
        {
            InitializeComponent();
        }

        public void Update(TelemetryData data)
        {
            Dispatcher.Invoke(() =>
            {
                float rpm = (float)data.RPM;
                float percentage = Math.Min(rpm / _maxRpm, 1.0f);
                
                RPMText.Text = $"{rpm:F0}";
                
                // Cor baseada na zona
                if (rpm >= _redlineRpm)
                {
                    RPMText.Foreground = Brushes.Red;
                    RPMText.FontWeight = FontWeights.Bold;
                    GaugeArc.Stroke = Brushes.Red;
                }
                else if (rpm >= _shiftLightRpm)
                {
                    RPMText.Foreground = Brushes.Orange;
                    RPMText.FontWeight = FontWeights.Normal;
                    GaugeArc.Stroke = Brushes.Orange;
                }
                else
                {
                    RPMText.Foreground = Brushes.White;
                    RPMText.FontWeight = FontWeights.Normal;
                    GaugeArc.Stroke = Brushes.LimeGreen;
                }
            });
        }
    }
}
