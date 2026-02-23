using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace RaceLabsOverlay
{
    /// <summary>
    /// Interface base para todos os widgets.
    /// </summary>
    public interface IWidget
    {
        string WidgetName { get; }
        Size DefaultSize { get; }
        bool IsConfigurable { get; }
        
        void Update(TelemetryData data);
        void Configure();
    }

    /// <summary>
    /// Dados de telemetria completos do iRacing.
    /// </summary>
    public class TelemetryData
    {
        // Timing
        public float SessionTime { get; set; }
        public int Lap { get; set; }
        public float LapCurrentLapTime { get; set; }
        public float LapLastLapTime { get; set; }
        public float LapBestLapTime { get; set; }
        public float LapDistPct { get; set; }
        
        // Delta
        public float DeltaToBest { get; set; }
        public bool IsDeltaValid { get; set; }
        
        // Car Data
        public float Speed { get; set; }
        public float RPM { get; set; }
        public int Gear { get; set; }
        public float Throttle { get; set; }
        public float Brake { get; set; }
        public float Clutch { get; set; }
        public float SteeringWheelAngle { get; set; }
        
        // Temps
        public float WaterTemp { get; set; }
        public float OilTemp { get; set; }
        
        // Tire Temps (12 valores - inner/middle/outer para cada pneu)
        public float TireTempLFInner { get; set; }
        public float TireTempLFMiddle { get; set; }
        public float TireTempLFOuter { get; set; }
        public float TireTempRFInner { get; set; }
        public float TireTempRFMiddle { get; set; }
        public float TireTempRFOuter { get; set; }
        public float TireTempLRInner { get; set; }
        public float TireTempLRMiddle { get; set; }
        public float TireTempLROuter { get; set; }
        public float TireTempRRInner { get; set; }
        public float TireTempRRMiddle { get; set; }
        public float TireTempRROuter { get; set; }
        
        // Fuel
        public float FuelLevel { get; set; }
        public float FuelLevelPct { get; set; }
        public float FuelUsePerHour { get; set; }
        public float FuelLapsRemaining { get; set; }
        public float FuelPerLap { get; set; }

        // Position
        public double PositionX { get; set; }
        public double PositionY { get; set; }
        public double PositionZ { get; set; }
        
        // Session
        public bool IsOnTrack { get; set; }
        public int SessionState { get; set; }
        
        // Computed Properties
        public float TireTempLFAvg => (TireTempLFInner + TireTempLFMiddle + TireTempLFOuter) / 3f;
        public float TireTempRFAvg => (TireTempRFInner + TireTempRFMiddle + TireTempRFOuter) / 3f;
        public float TireTempLRAvg => (TireTempLRInner + TireTempLRMiddle + TireTempLROuter) / 3f;
        public float TireTempRRAvg => (TireTempRRInner + TireTempRRMiddle + TireTempRROuter) / 3f;

        // Simple tire temp aliases (average of inner/middle/outer)
        public float TireTempLF => TireTempLFAvg;
        public float TireTempRF => TireTempRFAvg;
        public float TireTempLR => TireTempLRAvg;
        public float TireTempRR => TireTempRRAvg;
    }

    /// <summary>
    /// Gerenciador de widgets no overlay.
    /// </summary>
    public class WidgetManager
    {
        private readonly Canvas _canvas;
        private readonly List<WidgetContainer> _widgets = new();
        private bool _isEditMode = false;

        public WidgetManager(Canvas canvas)
        {
            _canvas = canvas;
        }

        public void AddWidget(IWidget widget, Point position)
        {
            var container = new WidgetContainer(widget, position);
            
            if (_isEditMode)
            {
                container.EnableDragging();
            }
            
            _widgets.Add(container);
            _canvas.Children.Add(container.Visual);
        }

        public void RemoveWidget(IWidget widget)
        {
            var container = _widgets.Find(w => w.Widget == widget);
            if (container != null)
            {
                _widgets.Remove(container);
                _canvas.Children.Remove(container.Visual);
            }
        }

        public void UpdateAllWidgets(TelemetryData data)
        {
            foreach (var container in _widgets)
            {
                container.Widget.Update(data);
            }
        }

        public void EnterEditMode()
        {
            _isEditMode = true;
            foreach (var container in _widgets)
            {
                container.EnableDragging();
                container.ShowBorder();
            }
        }

        public void ExitEditMode()
        {
            _isEditMode = false;
            foreach (var container in _widgets)
            {
                container.DisableDragging();
                container.HideBorder();
            }
            
            SaveLayout();
        }

        public void SaveLayout()
        {
            // TODO: Salvar posições dos widgets em JSON
        }

        public void LoadLayout()
        {
            // TODO: Carregar posições dos widgets
        }
    }

    /// <summary>
    /// Container visual para um widget.
    /// </summary>
    public class WidgetContainer
    {
        public IWidget Widget { get; }
        public UIElement Visual { get; }
        public Point Position { get; private set; }
        
        private Border _border;
        private bool _isDragging = false;
        private Point _dragStart;

        public WidgetContainer(IWidget widget, Point position)
        {
            Widget = widget;
            Position = position;
            
            // Criar container visual
            _border = new Border
            {
                BorderBrush = System.Windows.Media.Brushes.Transparent,
                BorderThickness = new Thickness(2),
                Child = (UIElement)widget,  // Assumindo que widget é um UserControl
                RenderTransform = new System.Windows.Media.TranslateTransform(position.X, position.Y)
            };
            
            Visual = _border;
        }

        public void EnableDragging()
        {
            Visual.MouseLeftButtonDown += OnMouseLeftButtonDown;
            Visual.MouseMove += OnMouseMove;
            Visual.MouseLeftButtonUp += OnMouseLeftButtonUp;
        }

        public void DisableDragging()
        {
            Visual.MouseLeftButtonDown -= OnMouseLeftButtonDown;
            Visual.MouseMove -= OnMouseMove;
            Visual.MouseLeftButtonUp -= OnMouseLeftButtonUp;
        }

        public void ShowBorder()
        {
            _border.BorderBrush = System.Windows.Media.Brushes.LimeGreen;
        }

        public void HideBorder()
        {
            _border.BorderBrush = System.Windows.Media.Brushes.Transparent;
        }

        private void OnMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _isDragging = true;
            _dragStart = e.GetPosition((UIElement)sender);
            ((UIElement)sender).CaptureMouse();
        }

        private void OnMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!_isDragging) return;
            
            var current = e.GetPosition((UIElement)sender);
            var offset = current - _dragStart;
            
            var transform = (System.Windows.Media.TranslateTransform)_border.RenderTransform;
            transform.X += offset.X;
            transform.Y += offset.Y;
        }

        private void OnMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _isDragging = false;
            ((UIElement)sender).ReleaseMouseCapture();
        }
    }
}
