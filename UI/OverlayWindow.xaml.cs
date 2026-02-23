using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Input;
using System.Windows.Media;
using RaceLabsOverlay.Services;

namespace RaceLabsOverlay.UI
{
    public partial class OverlayWindow : Window
    {
        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_LAYERED = 0x80000;
        private const int WS_EX_TRANSPARENT = 0x20;
        private const int WS_EX_TOOLWINDOW = 0x80;
        private const int WS_EX_NOACTIVATE = 0x8000000;

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter,
            int X, int Y, int cx, int cy, uint uFlags);

        private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        private const uint SWP_FRAMECHANGED = 0x20;
        private const uint SWP_NOMOVE = 0x2;
        private const uint SWP_NOSIZE = 0x1;
        private const uint SWP_NOACTIVATE = 0x10;

        private readonly WidgetManager _widgetManager;
        private readonly SettingsService _settingsService;
        private bool _isClickThrough = true;
        private bool _isEditMode = false;

        public WidgetManager WidgetManager => _widgetManager;

        public event Action? OnOpenWidgetPanel;

        public OverlayWindow(SettingsService settingsService)
        {
            InitializeComponent();

            _settingsService = settingsService;

            WindowStyle = WindowStyle.None;
            ResizeMode = ResizeMode.NoResize;
            Background = Brushes.Transparent;
            AllowsTransparency = true;
            ShowInTaskbar = false;
            Topmost = true;

            var screen = SystemParameters.WorkArea;
            Left = screen.Left;
            Top = screen.Top;
            Width = screen.Width;
            Height = screen.Height;

            _widgetManager = new WidgetManager(WidgetsCanvas);
            _widgetManager.SetSettingsService(settingsService);

            KeyDown += OverlayWindow_KeyDown;
            Loaded += OverlayWindow_Loaded;
        }

        private void OverlayWindow_Loaded(object sender, RoutedEventArgs e)
        {
            MakeOverlayWindow();
            RegisterAllWidgets();
            _widgetManager.LoadLayout();
        }

        private void RegisterAllWidgets()
        {
            // Core widgets (visible by default)
            _widgetManager.RegisterWidget(new Widgets.SpeedometerWidget(),
                new Point(Width / 2 - 100, Height - 200), defaultVisible: true);
            _widgetManager.RegisterWidget(new Widgets.DeltaBarWidget(),
                new Point(Width / 2 - 150, 50), defaultVisible: true);
            _widgetManager.RegisterWidget(new Widgets.RPMGaugeWidget(),
                new Point(Width - 220, Height - 220), defaultVisible: true);
            _widgetManager.RegisterWidget(new Widgets.LapTimerWidget(),
                new Point(Width - 250, 20), defaultVisible: true);
            _widgetManager.RegisterWidget(new Widgets.FuelWidget(),
                new Point(20, 20), defaultVisible: true);
            _widgetManager.RegisterWidget(new Widgets.InputsWidget(),
                new Point(20, Height / 2 - 100), defaultVisible: true);
            _widgetManager.RegisterWidget(new Widgets.TireTempsWidget(),
                new Point(20, Height - 180), defaultVisible: true);

            // Advanced widgets (hidden by default)
            _widgetManager.RegisterWidget(new Widgets.TelemetryGraphWidget(),
                new Point(Width / 2 - 250, Height - 280), defaultVisible: false);
            _widgetManager.RegisterWidget(new Widgets.TrackMapWidget(),
                new Point(Width - 220, Height / 2 - 100), defaultVisible: false);
            _widgetManager.RegisterWidget(new Widgets.GhostComparatorWidget(),
                new Point(Width / 2 - 200, 120), defaultVisible: false);
        }

        private void MakeOverlayWindow()
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            int exStyle = GetWindowLong(hwnd, GWL_EXSTYLE);

            exStyle |= WS_EX_LAYERED;
            exStyle |= WS_EX_TOOLWINDOW;
            exStyle |= WS_EX_NOACTIVATE;

            if (_isClickThrough)
                exStyle |= WS_EX_TRANSPARENT;

            SetWindowLong(hwnd, GWL_EXSTYLE, exStyle);
            SetWindowPos(hwnd, HWND_TOPMOST, 0, 0, 0, 0,
                SWP_FRAMECHANGED | SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE);
        }

        private void OverlayWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.O && Keyboard.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift))
            {
                ToggleEditMode();
                e.Handled = true;
            }

            if (e.Key == Key.H && Keyboard.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift))
            {
                ToggleClickThrough();
                e.Handled = true;
            }

            if (e.Key == Key.W && Keyboard.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift))
            {
                OnOpenWidgetPanel?.Invoke();
                e.Handled = true;
            }
        }

        private void ToggleEditMode()
        {
            _isEditMode = !_isEditMode;

            if (_isEditMode)
            {
                Background = new SolidColorBrush(Color.FromArgb(20, 0, 0, 0));
                _widgetManager.EnterEditMode();
                _isClickThrough = false;
                MakeOverlayWindow();
                ShowNotification("EDIT MODE - Drag widgets | X to close | Ctrl+Shift+O to exit");
            }
            else
            {
                Background = Brushes.Transparent;
                _widgetManager.ExitEditMode();
                _isClickThrough = true;
                MakeOverlayWindow();
                ShowNotification("Layout saved");
            }
        }

        public void ToggleEditModePublic()
        {
            ToggleEditMode();
        }

        private void ToggleClickThrough()
        {
            _isClickThrough = !_isClickThrough;
            MakeOverlayWindow();
            ShowNotification(_isClickThrough
                ? "Click-through: ON"
                : "Click-through: OFF");
        }

        private void ShowNotification(string message)
        {
            NotificationText.Text = message;
            NotificationBorder.Visibility = Visibility.Visible;

            var timer = new System.Windows.Threading.DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(3)
            };
            timer.Tick += (s, e) =>
            {
                NotificationBorder.Visibility = Visibility.Collapsed;
                timer.Stop();
            };
            timer.Start();
        }

        public void UpdateTelemetry(TelemetryData data)
        {
            _widgetManager.UpdateAllWidgets(data);
        }

        public void ShowConnectionStatus(bool connected)
        {
            ShowNotification(connected
                ? "iRacing CONNECTED - Live telemetry active"
                : "iRacing DISCONNECTED - Waiting for connection...");
        }
    }
}
