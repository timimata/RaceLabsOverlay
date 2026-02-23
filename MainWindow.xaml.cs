using System.Windows;
using System.Windows.Threading;
using RaceLabsOverlay.Services;

namespace RaceLabsOverlay
{
    public partial class MainWindow : Window
    {
        private readonly MockTelemetryProvider _mockProvider;
        private readonly DispatcherTimer _timer;

        public MainWindow()
        {
            InitializeComponent();

            _mockProvider = new MockTelemetryProvider();

            _timer = new DispatcherTimer
            {
                Interval = System.TimeSpan.FromMilliseconds(100)
            };
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        private void Timer_Tick(object? sender, System.EventArgs e)
        {
            var data = GenerateMockData();

            SpeedWidget.Update(data);
            RPMWidget.Update(data);
            LapWidget.Update(data);
            FuelWidget.Update(data);
            InputsWidget.Update(data);
            TireWidget.Update(data);
            TrackMapWidget.Update(data);
            DeltaWidget.Update(data);
            GhostWidget.Update(data);
            GraphWidget.Update(data);
        }

        private TelemetryData GenerateMockData()
        {
            var random = new System.Random();
            return new TelemetryData
            {
                Speed = random.Next(0, 300),
                RPM = random.Next(1000, 8000),
                Gear = random.Next(1, 7),
                Lap = random.Next(1, 20),
                LapCurrentLapTime = random.Next(80, 120),
                LapLastLapTime = random.Next(80, 120),
                LapBestLapTime = random.Next(75, 100),
                LapDistPct = (float)random.NextDouble(),
                DeltaToBest = (float)(random.NextDouble() * 2 - 1),
                IsDeltaValid = true,
                Throttle = (float)random.NextDouble() * 100f,
                Brake = (float)random.NextDouble() * 100f,
                Clutch = 0,
                TireTempLFInner = random.Next(60, 110),
                TireTempLFMiddle = random.Next(60, 110),
                TireTempLFOuter = random.Next(60, 110),
                TireTempRFInner = random.Next(60, 110),
                TireTempRFMiddle = random.Next(60, 110),
                TireTempRFOuter = random.Next(60, 110),
                TireTempLRInner = random.Next(60, 110),
                TireTempLRMiddle = random.Next(60, 110),
                TireTempLROuter = random.Next(60, 110),
                TireTempRRInner = random.Next(60, 110),
                TireTempRRMiddle = random.Next(60, 110),
                TireTempRROuter = random.Next(60, 110),
                FuelLevel = (float)(random.NextDouble() * 60),
                FuelLapsRemaining = (float)(random.NextDouble() * 20),
                FuelPerLap = (float)(random.NextDouble() * 5),
                IsOnTrack = true
            };
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            _timer?.Stop();
            _mockProvider?.Dispose();
            base.OnClosing(e);
        }
    }
}
