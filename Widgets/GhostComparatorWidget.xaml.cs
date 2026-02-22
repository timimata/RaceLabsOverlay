using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using RaceLabsOverlay.Services;

namespace RaceLabsOverlay.Widgets
{
    /// <summary>
    /// Widget que mostra a posição do carro vs ghost na pista.
    /// </summary>
    public partial class GhostComparatorWidget : UserControl, IWidget
    {
        private readonly GhostRecorder _ghostRecorder;
        
        public string WidgetName => "Ghost Comparator";
        public Size DefaultSize => new Size(400, 100);
        public bool IsConfigurable => false;
        
        // Cores
        private readonly Brush _playerColor = Brushes.LimeGreen;
        private readonly Brush _ghostColor = Brushes.Orange;
        private readonly Brush _aheadColor = Brushes.LimeGreen;
        private readonly Brush _behindColor = Brushes.Red;

        public GhostComparatorWidget(GhostRecorder ghostRecorder)
        {
            InitializeComponent();
            _ghostRecorder = ghostRecorder;
        }

        public void Update(TelemetryData data)
        {
            if (_ghostRecorder.BestLap == null)
            {
                // Esconder se não há ghost
                Visibility = Visibility.Collapsed;
                return;
            }
            
            Visibility = Visibility.Visible;
            
            // Comparar com ghost
            var comparison = _ghostRecorder.CompareWithGhost(
                data.LapDistPct, 
                data.LapCurrentLapTime);
            
            if (!comparison.HasGhost) return;
            
            Dispatcher.Invoke(() =>
            {
                // Atualizar delta
                DeltaText.Text = comparison.DeltaFormatted;
                DeltaText.Foreground = comparison.IsAhead ? _aheadColor : _behindColor;
                
                // Atualizar texto de status
                if (comparison.IsAhead)
                {
                    StatusText.Text = "AHEAD";
                    StatusText.Foreground = _aheadColor;
                }
                else
                {
                    StatusText.Text = "BEHIND";
                    StatusText.Foreground = _behindColor;
                }
                
                // Velocidade do ghost
                GhostSpeedText.Text = $"Ghost: {comparison.GhostSpeed:F0} km/h";
                
                // Barra visual de comparação
                UpdateComparisonBar(comparison.DeltaSeconds);
                
                // Indicadores de posição na pista
                UpdateTrackPosition(data.LapDistPct);
            });
        }

        private void UpdateComparisonBar(float delta)
        {
            // Barra central = 0 delta
            // Esquerda = ahead (negativo)
            // Direita = behind (positivo)
            
            double maxDelta = 2.0;  // 2 segundos = max visual
            double percentage = Math.Min(Math.Abs(delta) / maxDelta, 1.0);
            double barWidth = BarContainer.ActualWidth * percentage / 2;
            
            if (delta < 0)
            {
                // Ahead - barra à esquerda (verde)
                AheadBar.Width = barWidth;
                BehindBar.Width = 0;
                AheadBar.Fill = _aheadColor;
            }
            else
            {
                // Behind - barra à direita (vermelha)
                AheadBar.Width = 0;
                BehindBar.Width = barWidth;
                BehindBar.Fill = _behindColor;
            }
            
            // Mostrar seta indicadora
            ArrowIndicator.RenderTransform = new TranslateTransform(
                delta < 0 ? -20 : 20, 0);
        }

        private void UpdateTrackPosition(float lapDistPct)
        {
            // Mini representação da pista com posição do player e ghost
            double trackWidth = TrackRepresentation.ActualWidth;
            double playerPos = lapDistPct * trackWidth;
            
            PlayerMarker.RenderTransform = new TranslateTransform(playerPos, 0);
            
            // Ghost position (estimada baseada no delta)
            if (_ghostRecorder.BestLap != null)
            {
                // Simplificação: ghost está +/- delta na pista
                // Na realidade, precisaríamos de calcular baseado na speed
                float ghostDistPct = lapDistPct;  // TODO: Calcular corretamente
                double ghostPos = ghostDistPct * trackWidth;
                GhostMarker.RenderTransform = new TranslateTransform(ghostPos, 0);
            }
        }

        public void Configure()
        {
            // Não configurável
        }
    }
}
