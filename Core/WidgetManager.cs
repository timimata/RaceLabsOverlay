using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using RaceLabsOverlay.Services;

namespace RaceLabsOverlay
{
    public interface IWidget
    {
        string WidgetName { get; }
        Size DefaultSize { get; }
        bool IsConfigurable { get; }

        void Update(TelemetryData data);
        void Configure();
    }

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

        // Tire Temps
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

        public float TireTempLF => TireTempLFAvg;
        public float TireTempRF => TireTempRFAvg;
        public float TireTempLR => TireTempLRAvg;
        public float TireTempRR => TireTempRRAvg;
    }

    public class WidgetManager
    {
        private readonly Canvas _canvas;
        private readonly Dictionary<string, WidgetEntry> _registry = new();
        private readonly List<string> _registrationOrder = new();
        private SettingsService? _settingsService;
        private bool _isEditMode = false;

        public event Action<string, bool>? OnWidgetVisibilityChanged;

        public WidgetManager(Canvas canvas)
        {
            _canvas = canvas;
        }

        public void SetSettingsService(SettingsService service)
        {
            _settingsService = service;
        }

        public void RegisterWidget(IWidget widget, Point defaultPos, bool defaultVisible = true)
        {
            var container = new WidgetContainer(widget, defaultPos, this);
            _registry[widget.WidgetName] = new WidgetEntry
            {
                Widget = widget,
                Container = container,
                DefaultPosition = defaultPos,
                DefaultVisible = defaultVisible,
                IsVisible = false
            };
            _registrationOrder.Add(widget.WidgetName);
        }

        public void ShowWidget(string name)
        {
            if (!_registry.TryGetValue(name, out var entry) || entry.IsVisible)
                return;

            entry.IsVisible = true;
            if (_isEditMode)
            {
                entry.Container.EnableDragging();
                entry.Container.ShowEditControls();
            }
            _canvas.Children.Add(entry.Container.Visual);
            OnWidgetVisibilityChanged?.Invoke(name, true);
        }

        public void HideWidget(string name)
        {
            if (!_registry.TryGetValue(name, out var entry) || !entry.IsVisible)
                return;

            entry.IsVisible = false;
            if (_isEditMode)
            {
                entry.Container.DisableDragging();
                entry.Container.HideEditControls();
            }
            _canvas.Children.Remove(entry.Container.Visual);
            OnWidgetVisibilityChanged?.Invoke(name, false);
        }

        public bool IsWidgetVisible(string name)
        {
            return _registry.TryGetValue(name, out var entry) && entry.IsVisible;
        }

        public List<string> GetAllWidgetNames()
        {
            return _registrationOrder.ToList();
        }

        public void UpdateAllWidgets(TelemetryData data)
        {
            foreach (var entry in _registry.Values)
            {
                if (entry.IsVisible)
                    entry.Widget.Update(data);
            }
        }

        public void EnterEditMode()
        {
            _isEditMode = true;
            foreach (var entry in _registry.Values)
            {
                if (entry.IsVisible)
                {
                    entry.Container.EnableDragging();
                    entry.Container.ShowEditControls();
                }
            }
        }

        public void ExitEditMode()
        {
            _isEditMode = false;
            foreach (var entry in _registry.Values)
            {
                if (entry.IsVisible)
                {
                    entry.Container.DisableDragging();
                    entry.Container.HideEditControls();
                }
            }
            SaveLayout();
        }

        public void SaveLayout()
        {
            if (_settingsService == null) return;

            var layouts = new List<WidgetLayoutEntry>();
            foreach (var name in _registrationOrder)
            {
                var entry = _registry[name];
                var transform = (TranslateTransform)entry.Container.Visual.RenderTransform;
                layouts.Add(new WidgetLayoutEntry
                {
                    WidgetName = name,
                    IsVisible = entry.IsVisible,
                    X = transform.X,
                    Y = transform.Y
                });
            }
            _settingsService.Settings.WidgetLayouts = layouts;
            _settingsService.Save();
        }

        public void LoadLayout()
        {
            var layouts = _settingsService?.Settings.WidgetLayouts;

            if (layouts == null || layouts.Count == 0)
            {
                foreach (var name in _registrationOrder)
                {
                    if (_registry[name].DefaultVisible)
                        ShowWidget(name);
                }
                return;
            }

            foreach (var layout in layouts)
            {
                if (!_registry.TryGetValue(layout.WidgetName, out var entry))
                    continue;

                var transform = (TranslateTransform)entry.Container.Visual.RenderTransform;
                transform.X = layout.X;
                transform.Y = layout.Y;

                if (layout.IsVisible)
                    ShowWidget(layout.WidgetName);
            }
        }

        public void ResetLayout()
        {
            foreach (var name in _registrationOrder.ToList())
                HideWidget(name);

            foreach (var name in _registrationOrder)
            {
                var entry = _registry[name];
                var transform = (TranslateTransform)entry.Container.Visual.RenderTransform;
                transform.X = entry.DefaultPosition.X;
                transform.Y = entry.DefaultPosition.Y;

                if (entry.DefaultVisible)
                    ShowWidget(name);
            }
            SaveLayout();
        }

        private class WidgetEntry
        {
            public IWidget Widget { get; set; } = null!;
            public WidgetContainer Container { get; set; } = null!;
            public Point DefaultPosition { get; set; }
            public bool DefaultVisible { get; set; }
            public bool IsVisible { get; set; }
        }
    }

    public class WidgetContainer
    {
        public IWidget Widget { get; }
        public Grid Visual { get; }

        private readonly Border _border;
        private readonly Button _closeButton;
        private readonly TextBlock _nameLabel;
        private readonly WidgetManager _manager;
        private bool _isDragging = false;
        private Point _dragStart;

        public WidgetContainer(IWidget widget, Point position, WidgetManager manager)
        {
            Widget = widget;
            _manager = manager;

            _border = new Border
            {
                BorderBrush = Brushes.Transparent,
                BorderThickness = new Thickness(2),
                Child = (UIElement)widget
            };

            _closeButton = new Button
            {
                Content = "X",
                Width = 20,
                Height = 20,
                FontSize = 11,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(0, -22, 0, 0),
                Background = new SolidColorBrush(Color.FromArgb(200, 200, 30, 30)),
                Foreground = Brushes.White,
                BorderThickness = new Thickness(0),
                Cursor = System.Windows.Input.Cursors.Hand,
                Visibility = Visibility.Collapsed
            };
            _closeButton.Click += (s, e) => _manager.HideWidget(widget.WidgetName);

            _nameLabel = new TextBlock
            {
                Text = widget.WidgetName,
                Foreground = Brushes.LimeGreen,
                FontSize = 10,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(2, -16, 0, 0),
                Visibility = Visibility.Collapsed
            };

            Visual = new Grid
            {
                RenderTransform = new TranslateTransform(position.X, position.Y)
            };
            Visual.Children.Add(_border);
            Visual.Children.Add(_closeButton);
            Visual.Children.Add(_nameLabel);
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

        public void ShowEditControls()
        {
            _border.BorderBrush = Brushes.LimeGreen;
            _closeButton.Visibility = Visibility.Visible;
            _nameLabel.Visibility = Visibility.Visible;
        }

        public void HideEditControls()
        {
            _border.BorderBrush = Brushes.Transparent;
            _closeButton.Visibility = Visibility.Collapsed;
            _nameLabel.Visibility = Visibility.Collapsed;
        }

        private void OnMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _isDragging = true;
            _dragStart = e.GetPosition(Visual);
            Visual.CaptureMouse();
            e.Handled = true;
        }

        private void OnMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!_isDragging) return;

            var current = e.GetPosition(Visual);
            var offset = current - _dragStart;

            var transform = (TranslateTransform)Visual.RenderTransform;
            transform.X += offset.X;
            transform.Y += offset.Y;
        }

        private void OnMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _isDragging = false;
            Visual.ReleaseMouseCapture();
            e.Handled = true;
        }
    }
}
