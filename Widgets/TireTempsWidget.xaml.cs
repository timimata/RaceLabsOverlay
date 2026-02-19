using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace RaceLabsOverlay.Widgets
{
    /// <summary>
    /// Widget de temperaturas dos pneus com gradientes de cor.
    /// </summary>
    public partial class TireTempsWidget : UserControl, IWidget
    {
        public string WidgetName => "Tire Temperatures";
        public Size DefaultSize => new Size(200, 150);
        public bool IsConfigurable => true;
        
        // Temperaturas ideais (configuráveis por pista/carro)
        private float _optimalTemp = 85f;  // ºC
        private float _minTemp = 60f;
        private float _maxTemp = 110f;
        
        public TireTempsWidget()
        {
            InitializeComponent();
            
            // Valores iniciais
            Update(new TelemetryData());  // Zeros
        }

        public void Update(TelemetryData data)
        {
            Dispatcher.Invoke(() =>
            {
                // Atualizar Left Front
                UpdateTireDisplay(LFInner, LFOuter, LFCenter, LFAvg, 
                    data.TireTempLFInner, data.TireTempLFOuter, data.TireTempLFCenter);
                
                // Atualizar Right Front
                UpdateTireDisplay(RFInner, RFOuter, RFCenter, RFAvg,
                    data.TireTempRFInner, data.TireTempRFOuter, data.TireTempRFCenter);
                
                // Atualizar Left Rear
                UpdateTireDisplay(LRInner, LROuter, LRCenter, LRAvg,
                    data.TireTempLRInner, data.TireTempLROuter, data.TireTempLRCenter);
                
                // Atualizar Right Rear
                UpdateTireDisplay(RRInner, RROuter, RRCenter, RRAvg,
                    data.TireTempRRInner, data.TireTempRROuter, data.TireTempRRCenter);
            });
        }

        private void UpdateTireDisplay(
            Border innerBorder, Border outerBorder, Border centerBorder, TextBlock avgText,
            float innerTemp, float outerTemp, float centerTemp)
        {
            // Calcular média
            float avg = (innerTemp + outerTemp + centerTemp) / 3f;
            avgText.Text = $"{avg:F0}°";
            
            // Cor baseada na temperatura
            Brush color = GetTempColor(avg);
            avgText.Foreground = color;
            
            // Atualizar cores das três zonas
            innerBorder.Background = GetTempBrush(innerTemp);
            outerBorder.Background = GetTempBrush(outerTemp);
            centerBorder.Background = GetTempBrush(centerTemp);
            
            // Indicador de desgaste (simplificado)
            // Verde = bom, Amarelo = atenção, Vermelho = perigo
        }

        private Brush GetTempBrush(float temp)
        {
            if (temp < _minTemp)
                return new SolidColorBrush(Color.FromRgb(0, 100, 255));  // Azul - frio
            if (temp > _maxTemp)
                return new SolidColorBrush(Color.FromRgb(255, 0, 0));    // Vermelho - quente
            if (Math.Abs(temp - _optimalTemp) < 10)
                return new SolidColorBrush(Color.FromRgb(0, 255, 0));    // Verde - ideal
            if (temp < _optimalTemp)
                return new SolidColorBrush(Color.FromRgb(0, 255, 255));  // Ciano - abaixo do ideal
            
            return new SolidColorBrush(Color.FromRgb(255, 255, 0));      // Amarelo - acima do ideal
        }

        private Brush GetTempColor(float temp)
        {
            // Versão simplificada para texto
            if (Math.Abs(temp - _optimalTemp) < 10)
                return Brushes.LimeGreen;
            if (temp < _minTemp || temp > _maxTemp)
                return Brushes.Red;
            return Brushes.Yellow;
        }

        public void Configure()
        {
            // Configurar temperaturas ideais
            // TODO: Implementar
        }
    }
}
