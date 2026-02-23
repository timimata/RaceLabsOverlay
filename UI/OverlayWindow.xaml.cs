using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Input;
using System.Windows.Media;

namespace RaceLabsOverlay.UI
{
    /// <summary>
    /// Janela de overlay transparente que fica sempre no topo do iRacing.
    /// </summary>
    public partial class OverlayWindow : Window
    {
        // Win32 APIs para tornar a janela click-through e sempre no topo
        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_LAYERED = 0x80000;
        private const int WS_EX_TRANSPARENT = 0x20;
        private const int WS_EX_TOOLWINDOW = 0x80;  // Não aparece na taskbar
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
        private const uint SWP_SHOWWINDOW = 0x40;
        private const uint SWP_NOACTIVATE = 0x10;

        private WidgetManager _widgetManager;
        private bool _isClickThrough = true;
        private bool _isEditMode = false;

        public OverlayWindow()
        {
            InitializeComponent();
            
            // Configurações da janela
            WindowStyle = WindowStyle.None;
            ResizeMode = ResizeMode.NoResize;
            Background = Brushes.Transparent;
            AllowsTransparency = true;
            ShowInTaskbar = false;
            Topmost = true;
            
            // Tamanho e posição (monitor primário full screen)
            var screen = SystemParameters.WorkArea;
            Left = screen.Left;
            Top = screen.Top;
            Width = screen.Width;
            Height = screen.Height;
            
            // Inicializar widget manager
            _widgetManager = new WidgetManager(WidgetsCanvas);
            
            // Atalhos de teclado
            KeyDown += OverlayWindow_KeyDown;
            
            Loaded += OverlayWindow_Loaded;
        }

        private void OverlayWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Aplicar estilos de overlay após a janela ser carregada
            MakeOverlayWindow();
            
            // Carregar widgets default
            LoadDefaultWidgets();
        }

        private void MakeOverlayWindow()
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            
            // Obter estilos atuais
            int exStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            
            // Adicionar estilos de overlay
            exStyle |= WS_EX_LAYERED;
            exStyle |= WS_EX_TOOLWINDOW;
            exStyle |= WS_EX_NOACTIVATE;
            
            if (_isClickThrough)
            {
                exStyle |= WS_EX_TRANSPARENT;
            }
            
            // Aplicar novos estilos
            SetWindowLong(hwnd, GWL_EXSTYLE, exStyle);
            
            // Forçar atualização
            SetWindowPos(hwnd, HWND_TOPMOST, 0, 0, 0, 0,
                SWP_FRAMECHANGED | SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE);
        }

        private void LoadDefaultWidgets()
        {
            // Speedometer - centro baixo
            var speedo = new Widgets.SpeedometerWidget();
            _widgetManager.AddWidget(speedo, new Point(Width / 2 - 100, Height - 200));
            
            // Delta Bar - topo centro
            var delta = new Widgets.DeltaBarWidget();
            _widgetManager.AddWidget(delta, new Point(Width / 2 - 150, 50));
            
            // Tire Temps - canto inferior esquerdo
            var tires = new Widgets.TireTempsWidget();
            _widgetManager.AddWidget(tires, new Point(20, Height - 180));
            
            // RPM Gauge - canto inferior direito
            var rpm = new Widgets.RPMGaugeWidget();
            _widgetManager.AddWidget(rpm, new Point(Width - 220, Height - 220));
            
            // Lap Timer - topo direito
            var lapTimer = new Widgets.LapTimerWidget();
            _widgetManager.AddWidget(lapTimer, new Point(Width - 250, 20));
            
            // Inputs - esquerda centro
            var inputs = new Widgets.InputsWidget();
            _widgetManager.AddWidget(inputs, new Point(20, Height / 2 - 100));
            
            // Fuel - topo esquerdo
            var fuel = new Widgets.FuelWidget();
            _widgetManager.AddWidget(fuel, new Point(20, 20));
            
            // Track Map - direita centro
            var trackMap = new Widgets.TrackMapWidget();
            _widgetManager.AddWidget(trackMap, new Point(Width - 220, Height / 2 - 100));

            // Ghost Comparator - centro
            var ghost = new Widgets.GhostComparatorWidget();
            _widgetManager.AddWidget(ghost, new Point(Width / 2 - 200, 120));

            // Standings - topo esquerdo abaixo do fuel
            var standings = new Widgets.StandingsWidget();
            _widgetManager.AddWidget(standings, new Point(20, 160));

            // Sector Times - topo centro-esquerdo
            var sectors = new Widgets.SectorTimesWidget();
            _widgetManager.AddWidget(sectors, new Point(220, 20));

            // Relative - esquerda abaixo standings
            var relative = new Widgets.RelativeWidget();
            _widgetManager.AddWidget(relative, new Point(20, 300));

            // Minimap - direita abaixo track map
            var minimap = new Widgets.MinimapWidget();
            _widgetManager.AddWidget(minimap, new Point(Width - 220, Height / 2 + 120));

            // Telemetry Graph - centro baixo
            var graph = new Widgets.TelemetryGraphWidget();
            _widgetManager.AddWidget(graph, new Point(Width / 2 - 250, Height - 280));
        }

        private void OverlayWindow_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            // Ctrl + Shift + O = Toggle overlay mode
            if (e.Key == System.Windows.Input.Key.O &&
                Keyboard.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift))
            {
                ToggleEditMode();
                e.Handled = true;
            }
            
            // Ctrl + Shift + H = Toggle click-through
            if (e.Key == System.Windows.Input.Key.H &&
                Keyboard.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift))
            {
                ToggleClickThrough();
                e.Handled = true;
            }
        }

        private void ToggleEditMode()
        {
            _isEditMode = !_isEditMode;
            
            if (_isEditMode)
            {
                // Modo edição: mostrar grid, permitir mover widgets
                Background = new SolidColorBrush(Color.FromArgb(20, 0, 0, 0));
                _widgetManager.EnterEditMode();
                
                // Tornar clicável para poder editar
                _isClickThrough = false;
                MakeOverlayWindow();
                
                ShowNotification("EDIT MODE - Drag widgets to reposition");
            }
            else
            {
                // Modo normal: transparente, widgets fixos
                Background = Brushes.Transparent;
                _widgetManager.ExitEditMode();
                
                // Voltar a click-through
                _isClickThrough = true;
                MakeOverlayWindow();
                
                ShowNotification("OVERLAY MODE - Ctrl+Shift+H to toggle click-through");
            }
        }

        private void ToggleClickThrough()
        {
            _isClickThrough = !_isClickThrough;
            MakeOverlayWindow();
            
            ShowNotification(_isClickThrough 
                ? "Click-through: ON (passes clicks to game)" 
                : "Click-through: OFF (interact with overlay)");
        }

        private void ShowNotification(string message)
        {
            NotificationText.Text = message;
            NotificationBorder.Visibility = Visibility.Visible;
            
            // Fade out após 3 segundos
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
            if (connected)
            {
                ShowNotification("iRacing CONNECTED - Live telemetry active");
            }
            else
            {
                ShowNotification("iRacing DISCONNECTED - Waiting for connection...");
            }
        }
    }
}
