using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace RaceLabsOverlay.Widgets
{
    /// <summary>
    /// Widget que mostra o delta (diferença) para a melhor volta em tempo real.
    /// </summary>
    public partial class DeltaBarWidget : UserControl, IWidget
    {
        private double _currentDelta = 0;
        private bool _isDeltaValid = false;
        
        public string WidgetName => "Delta Bar";
        public Size DefaultSize => new Size(300, 60);
        public bool IsConfigurable => true;
        
        public DeltaBarWidget()
        {
            InitializeComponent();
            
            // Valores iniciais
            UpdateDisplay(0, false);
        }

        public void Update(TelemetryData data)
        {
            if (data.IsDeltaValid)
            {
                UpdateDisplay(data.DeltaToBest, true);
            }
            else
            {
                UpdateDisplay(0, false);
            }
        }

        private void UpdateDisplay(float delta, bool isValid)
        {
            _currentDelta = delta;
            _isDeltaValid = isValid;
            
            Dispatcher.Invoke(() =>
            {
                if (!isValid)
                {
                    // Sem delta válido
                    DeltaText.Text = "—.——";
                    DeltaText.Foreground = Brushes.Gray;
                    PositiveBar.Width = 0;
                    NegativeBar.Width = 0;
                    CenterMarker.Visibility = Visibility.Collapsed;
                    return;
                }
                
                CenterMarker.Visibility = Visibility.Visible;
                
                // Formatar texto
                string sign = delta >= 0 ? "+" : "";
                DeltaText.Text = $"{sign}{delta:F2}";
                
                // Cor baseada no delta
                if (delta <= 0)
                {
                    // Mais rápido = verde
                    DeltaText.Foreground = Brushes.LimeGreen;
                    PositiveBar.Fill = Brushes.LimeGreen;
                }
                else
                {
                    // Mais lento = vermelho/laranja
                    DeltaText.Foreground = delta > 1.0 ? Brushes.Red : Brushes.Orange;
                    PositiveBar.Fill = delta > 1.0 ? Brushes.Red : Brushes.Orange;
                }
                
                // Atualizar barras
                double maxBarWidth = BarContainer.ActualWidth / 2 - 10;
                double barScale = Math.Min(Math.Abs(delta) * 100, maxBarWidth);  // Escala: 1s = 100px
                
                if (delta > 0)
                {
                    // Positivo - barra à direita
                    PositiveBar.Width = barScale;
                    NegativeBar.Width = 0;
                }
                else if (delta < 0)
                {
                    // Negativo - barra à esquerda
                    NegativeBar.Width = barScale;
                    PositiveBar.Width = 0;
                }
                else
                {
                    // Zero
                    PositiveBar.Width = 0;
                    NegativeBar.Width = 0;
                }
                
                // Animação suave
                AnimateBarChange();
            });
        }

        private void AnimateBarChange()
        {
            // Adicionar efeito de pulso quando o delta muda significativamente
            var storyboard = new Storyboard();
            
            var fade = new DoubleAnimation
            {
                From = 1.0,
                To = 0.7,
                Duration = TimeSpan.FromMilliseconds(100),
                AutoReverse = true
            };
            
            Storyboard.SetTarget(fade, DeltaText);
            Storyboard.SetTargetProperty(fade, new PropertyPath(OpacityProperty));
            
            storyboard.Children.Add(fade);
            storyboard.Begin();
        }

        public void Configure()
        {
            // Abrir janela de configuração
            // TODO: Implementar configuração de cores, escala, etc.
        }
    }
}
