using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RaceLabsOverlay.Widgets
{
    /// <summary>
    /// Widget que mostra a gap para carros à frente e atrás (Relative).
    /// </summary>
    public partial class RelativeWidget : UserControl, IWidget
    {
        public string WidgetName => "Relative";
        public Size DefaultSize => new Size(220, 180);
        public bool IsConfigurable => false;
        
        // Dados dos carros (simplificado)
        private List<CarRelativeData> _relativeData = new();

        public RelativeWidget()
        {
            InitializeComponent();
        }

        public void Update(TelemetryData data)
        {
            // Simular dados (na implementação real, ler do iRacing)
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            Dispatcher.Invoke(() =>
            {
                // Carro à frente (P1)
                Position1Text.Text = "P1";
                Car1Text.Text = "#24";
                Gap1Text.Text = "+0.234";
                Gap1Text.Foreground = new SolidColorBrush(Color.FromRgb(255, 100, 100)); // Vermelho (mais lento que tu)
                
                // O teu carro (P2)
                Position2Text.Text = "P2";
                Car2Text.Text = "YOU";
                Gap2Text.Text = "——";
                Gap2Text.Foreground = Brushes.LimeGreen;
                Background2.Opacity = 0.3; // Highlight
                
                // Carro atrás (P3)
                Position3Text.Text = "P3";
                Car3Text.Text = "#07";
                Gap3Text.Text = "-0.567";
                Gap3Text.Foreground = new SolidColorBrush(Color.FromRgb(100, 255, 100)); // Verde (mais rápido)
                
                // Info adicional
                TotalCarsText.Text = "/ 24 cars";
                LapsToLeaderText.Text = "+2 laps to leader";
            });
        }

        public void Configure() { }
    }

    public class CarRelativeData
    {
        public int Position { get; set; }
        public int CarNumber { get; set; }
        public string DriverName { get; set; } = "";
        public float GapToPlayer { get; set; } // Positivo = à frente, Negativo = atrás
        public bool IsPlayer { get; set; }
    }
}
