using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace RaceLabsOverlay.Widgets
{
    /// <summary>
    /// Widget de gráficos de telemetria em tempo real.
    /// </summary>
    public partial class TelemetryGraphWidget : UserControl, IWidget
    {
        public string WidgetName => "Telemetry Graphs";
        public Size DefaultSize => new Size(500, 300);
        public bool IsConfigurable => true;

        // Dados para os gráficos
        private readonly List<ObservablePoint> _speedData = new();
        private readonly List<ObservablePoint> _rpmData = new();
        private readonly List<ObservablePoint> _throttleData = new();
        private readonly List<ObservablePoint> _brakeData = new();
        
        private const int MaxDataPoints = 300; // 5 segundos a 60fps
        private double _timeCounter = 0;

        public TelemetryGraphWidget()
        {
            InitializeComponent();
            SetupCharts();
        }

        private void SetupCharts()
        {
            // Configurar gráfico de Speed
            SpeedChart.Series = new List<ISeries>
            {
                new LineSeries<ObservablePoint>
                {
                    Values = _speedData,
                    Stroke = new SolidColorPaint(SKColors.LimeGreen) { StrokeThickness = 2 },
                    Fill = null,
                    GeometrySize = 0,
                    LineSmoothness = 0.3
                }
            };
            
            SpeedChart.XAxes = new List<Axis>
            {
                new Axis { IsVisible = false }
            };
            
            SpeedChart.YAxes = new List<Axis>
            {
                new Axis 
                { 
                    MinLimit = 0, 
                    MaxLimit = 350,
                    TextPaint = new SolidColorPaint(SKColors.Gray),
                    LabelsPaint = new SolidColorPaint(SKColors.Gray)
                }
            };

            // Configurar gráfico de RPM
            RpmChart.Series = new List<ISeries>
            {
                new LineSeries<ObservablePoint>
                {
                    Values = _rpmData,
                    Stroke = new SolidColorPaint(SKColors.Orange) { StrokeThickness = 2 },
                    Fill = null,
                    GeometrySize = 0,
                    LineSmoothness = 0.3
                }
            };
            
            RpmChart.YAxes = new List<Axis>
            {
                new Axis 
                { 
                    MinLimit = 0, 
                    MaxLimit = 9000,
                    TextPaint = new SolidColorPaint(SKColors.Gray),
                    LabelsPaint = new SolidColorPaint(SKColors.Gray)
                }
            };

            // Configurar gráfico de Inputs
            InputsChart.Series = new List<ISeries>
            {
                new LineSeries<ObservablePoint>
                {
                    Values = _throttleData,
                    Stroke = new SolidColorPaint(SKColors.LimeGreen) { StrokeThickness = 2 },
                    Fill = new SolidColorPaint(SKColors.LimeGreen.WithAlpha(50)),
                    GeometrySize = 0
                },
                new LineSeries<ObservablePoint>
                {
                    Values = _brakeData,
                    Stroke = new SolidColorPaint(SKColors.Red) { StrokeThickness = 2 },
                    Fill = new SolidColorPaint(SKColors.Red.WithAlpha(50)),
                    GeometrySize = 0
                }
            };
            
            InputsChart.YAxes = new List<Axis>
            {
                new Axis 
                { 
                    MinLimit = 0, 
                    MaxLimit = 100,
                    TextPaint = new SolidColorPaint(SKColors.Gray),
                    LabelsPaint = new SolidColorPaint(SKColors.Gray)
                }
            };
        }

        public void Update(TelemetryData data)
        {
            _timeCounter += 0.016; // ~60fps
            
            // Adicionar novos pontos
            _speedData.Add(new ObservablePoint(_timeCounter, data.Speed));
            _rpmData.Add(new ObservablePoint(_timeCounter, data.RPM));
            _throttleData.Add(new ObservablePoint(_timeCounter, data.Throttle));
            _brakeData.Add(new ObservablePoint(_timeCounter, data.Brake));
            
            // Manter apenas os últimos pontos
            if (_speedData.Count > MaxDataPoints)
            {
                _speedData.RemoveAt(0);
                _rpmData.RemoveAt(0);
                _throttleData.RemoveAt(0);
                _brakeData.RemoveAt(0);
                
                // Renormalizar tempo
                var offset = _speedData[0].X;
                foreach (var point in _speedData) point.X -= offset;
                foreach (var point in _rpmData) point.X -= offset;
                foreach (var point in _throttleData) point.X -= offset;
                foreach (var point in _brakeData) point.X -= offset;
                
                _timeCounter -= offset;
            }
            
            // Atualizar UI
            Dispatcher.Invoke(() =>
            {
                SpeedChart.Update();
                RpmChart.Update();
                InputsChart.Update();
                
                // Atualizar valores atuais
                CurrentSpeedText.Text = $"{data.Speed:F0} km/h";
                CurrentRpmText.Text = $"{data.RPM:F0} RPM";
                CurrentGearText.Text = $"Gear {data.Gear}";
            });
        }

        public void Configure()
        {
            // TODO: Abrir configurações de gráficos
        }
    }
}
