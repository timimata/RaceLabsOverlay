using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RaceLabsOverlay.Widgets
{
    /// <summary>
    /// Widget de RPM com gauge circular e shift lights.
    /// </summary>
    public partial class RPMGaugeWidget : UserControl, IWidget
    {
        public string WidgetName => "RPM Gauge";
        public Size DefaultSize => new Size(200, 200);
        public bool IsConfigurable => true;
        
        // Configurações (idealmente carregadas do setup do carro)
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
                float rpm = data.RPM;
                float percentage = Math.Min(rpm / _maxRpm, 1.0f);
                
                // Atualizar texto
                RPMText.Text = $"{rpm:F0}";
                
                // Atualizar arco do gauge
                UpdateGaugeArc(percentage);
                
                // Shift lights
                UpdateShiftLights(rpm);
                
                // Cor do texto muda no redline
                if (rpm >= _redlineRpm)
                {
                    RPMText.Foreground = Brushes.Red;
                    RPMText.FontWeight = FontWeights.Bold;
                }
                else if (rpm >= _shiftLightRpm)
                {
                    RPMText.Foreground = Brushes.Orange;
                    RPMText.FontWeight = FontWeights.Normal;
                }
                else
                {
                    RPMText.Foreground = Brushes.White;
                    RPMText.FontWeight = FontWeights.Normal;
                }
            });
        }

        private void UpdateGaugeArc(float percentage)
        {
            // Path do arco - implementação simplificada
            // Em WPF real, usaríamos ArcSegment ou uma biblioteca como LiveCharts
            
            double angle = percentage * 270 - 135;  // -135 a +135 graus
            
            // Atualizar cor baseada na zona
            if (percentage >= _redlineRpm / _maxRpm)
                GaugeArc.Stroke = Brushes.Red;
            else if (percentage >= _shiftLightRpm / _maxRpm)
                GaugeArc.Stroke = Brushes.Orange;
            else
                GaugeArc.Stroke = Brushes.LimeGreen;
        }

        private void UpdateShiftLights(float rpm)
        {
            // Sistema de shift lights (LEDs)
            int numLights = 10;
            float rpmPerLight = (_redlineRpm - _shiftLightRpm) / numLights;
            
            for (int i = 0; i < numLights; i++)
            {
                float threshold = _shiftLightRpm + (i * rpmPerLight);
                bool isLit = rpm >= threshold;
                
                // Atualizar UI dos LEDs
                // Light1, Light2, etc. seriam Ellipses ou Rectangles no XAML
                UpdateLight(i, isLit, rpm >= _redlineRpm);
            }
        }

        private void UpdateLight(int index, bool isLit, bool isRedline)
        {
            // Implementação simplificada
            // Encontrar o controle do LED pelo nome
            var light = FindName($"Light{index + 1}") as System.Windows.Shapes.Ellipse;
            if (light != null)
            {
                light.Fill = isLit 
                    ? (isRedline ? Brushes.Red : Brushes.LimeGreen)
                    : Brushes.Gray;
                light.Opacity = isLit ? 1.0 : 0.3;
            }
        }

        public void Configure()
        {
            // Configurar RPMs do carro
            // TODO: Implementar
        }
    }
}
