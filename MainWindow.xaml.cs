using System.Windows;
using System.Windows.Threading;
using RaceLabsOverlay.Services;
using RaceLabsOverlay.Widgets;

namespace RaceLabsOverlay
{
    public partial class MainWindow : Window
    {
        private readonly ITelemetryProvider _telemetry;
        private readonly DispatcherTimer _timer;

        public MainWindow()
        {
            InitializeComponent();
            
            // Usar mock provider para testes
            _telemetry = new MockTelemetryProvider();
            
            // Timer para atualizar a cada 100ms
            _timer = new DispatcherTimer
            {
                Interval = System.TimeSpan.FromMilliseconds(100)
            };
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        private void Timer_Tick(object? sender, System.EventArgs e)
        {
            var data = _telemetry.GetData();
            
            // Update all widgets
            SpeedWidget.UpdateSpeed(data.Speed);
            RPMWidget.Update(data);
            LapWidget.Update(data);
            FuelWidget.Update(data);
            InputsWidget.Update(data);
            TireWidget.Update(data);
            StandingsWidget.Update(data);
            SectorWidget.Update(data);
            RelativeWidget.Update(data);
            TrackMapWidget.Update(data);
            MinimapWidget.Update(data);
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            _timer?.Stop();
            base.OnClosing(e);
        }
    }
}
