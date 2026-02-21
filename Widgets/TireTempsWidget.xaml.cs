using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RaceLabsOverlay.Widgets
{
    public partial class TireTempsWidget : UserControl
    {
        public string WidgetName => "Tire Temperatures";
        public Size DefaultSize => new Size(200, 150);
        
        private float _optimalTemp = 85f;
        private float _minTemp = 60f;
        private float _maxTemp = 110f;
        
        public TireTempsWidget()
        {
            InitializeComponent();
        }

        public void Update(TelemetryData data)
        {
            Dispatcher.Invoke(() =>
            {
                UpdateTireDisplay(LFTire, LFTemp, data.TireTempLF, "LF");
                UpdateTireDisplay(RFTire, RFTemp, data.TireTempRF, "RF");
                UpdateTireDisplay(LRTire, LRTemp, data.TireTempLR, "LR");
                UpdateTireDisplay(RRTire, RRTemp, data.TireTempRR, "RR");
            });
        }

        private void UpdateTireDisplay(Border tireBorder, TextBlock tempText, float temp, string label)
        {
            tempText.Text = $"{label}: {temp:F0}°";
            tempText.Foreground = GetTempColor(temp);
            tireBorder.Background = GetTempBrush(temp);
        }

        private Brush GetTempBrush(float temp)
        {
            if (temp < _minTemp)
                return new SolidColorBrush(Color.FromRgb(0, 100, 255));
            if (temp > _maxTemp)
                return new SolidColorBrush(Color.FromRgb(255, 0, 0));
            if (Math.Abs(temp - _optimalTemp) < 10)
                return new SolidColorBrush(Color.FromRgb(0, 255, 0));
            if (temp < _optimalTemp)
                return new SolidColorBrush(Color.FromRgb(0, 255, 255));
            
            return new SolidColorBrush(Color.FromRgb(255, 255, 0));
        }

        private Brush GetTempColor(float temp)
        {
            if (Math.Abs(temp - _optimalTemp) < 10)
                return Brushes.LimeGreen;
            if (temp < _minTemp || temp > _maxTemp)
                return Brushes.Red;
            return Brushes.Yellow;
        }
    }
}
