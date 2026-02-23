using System;
using System.Threading;
using System.Threading.Tasks;
using RaceLabsOverlay.Core.Telemetry;

namespace RaceLabsOverlay.Services
{
    /// <summary>
    /// Mock telemetry provider for testing widgets without iRacing.
    /// </summary>
    public class MockTelemetryProvider : ITelemetryProvider, IDisposable
    {
        private readonly Random _random = new();
        private CancellationTokenSource? _cts;
        private Task? _pollingTask;

        public bool IsConnected => true;
        public event EventHandler<TelemetryData>? OnTelemetryUpdated;
        public event EventHandler? OnConnected;
        public event EventHandler? OnDisconnected;

        public Task StartAsync(CancellationToken cancellationToken = default)
        {
            _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            OnConnected?.Invoke(this, EventArgs.Empty);
            _pollingTask = PollingLoopAsync(_cts.Token);
            return Task.CompletedTask;
        }

        public async Task StopAsync()
        {
            _cts?.Cancel();
            if (_pollingTask != null)
            {
                try { await _pollingTask; }
                catch (OperationCanceledException) { }
            }
        }

        private async Task PollingLoopAsync(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                var data = GenerateMockData();
                OnTelemetryUpdated?.Invoke(this, data);
                await Task.Delay(100, ct);
            }
        }

        private TelemetryData GenerateMockData()
        {
            return new TelemetryData
            {
                // Timing
                SessionTime = (float)_random.NextDouble() * 3600,
                Lap = _random.Next(1, 20),
                LapCurrentLapTime = _random.Next(80, 120),
                LapLastLapTime = _random.Next(80, 120),
                LapBestLapTime = _random.Next(75, 100),
                LapDistPct = (float)_random.NextDouble(),

                // Delta
                DeltaToBest = (float)(_random.NextDouble() * 2 - 1),
                IsDeltaValid = true,

                // Car
                Speed = _random.Next(0, 300),
                RPM = _random.Next(1000, 8000),
                Gear = _random.Next(1, 7),
                Throttle = (float)_random.NextDouble() * 100f,
                Brake = (float)_random.NextDouble() * 100f,
                Clutch = 0,
                SteeringWheelAngle = (float)(_random.NextDouble() * 2 - 1),

                // Temps
                WaterTemp = _random.Next(80, 110),
                OilTemp = _random.Next(90, 120),

                // Tire Temps
                TireTempLFInner = _random.Next(60, 110),
                TireTempLFMiddle = _random.Next(60, 110),
                TireTempLFOuter = _random.Next(60, 110),
                TireTempRFInner = _random.Next(60, 110),
                TireTempRFMiddle = _random.Next(60, 110),
                TireTempRFOuter = _random.Next(60, 110),
                TireTempLRInner = _random.Next(60, 110),
                TireTempLRMiddle = _random.Next(60, 110),
                TireTempLROuter = _random.Next(60, 110),
                TireTempRRInner = _random.Next(60, 110),
                TireTempRRMiddle = _random.Next(60, 110),
                TireTempRROuter = _random.Next(60, 110),

                // Fuel
                FuelLevel = (float)(_random.NextDouble() * 60),
                FuelLevelPct = (float)_random.NextDouble(),
                FuelUsePerHour = (float)(_random.NextDouble() * 10),
                FuelLapsRemaining = (float)(_random.NextDouble() * 20),
                FuelPerLap = (float)(_random.NextDouble() * 5),

                // Position
                PositionX = _random.NextDouble() * 1000,
                PositionY = _random.NextDouble() * 100,
                PositionZ = _random.NextDouble() * 1000,

                // Session
                IsOnTrack = true,
                SessionState = 4
            };
        }

        public void Dispose()
        {
            StopAsync().GetAwaiter().GetResult();
            _cts?.Dispose();
        }
    }
}
