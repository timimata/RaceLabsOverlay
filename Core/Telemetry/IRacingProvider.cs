using irsdkSharp;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RaceLabsOverlay.Core.Telemetry
{
    /// <summary>
    /// Provider de telemetria para iRacing usando irsdkSharp.
    /// </summary>
    public class IRacingProvider : ITelemetryProvider, IDisposable
    {
        private readonly IRacingSDK _sdk;
        private CancellationTokenSource? _cts;
        private Task? _pollingTask;
        private bool _disposed;
        private bool _wasConnected;

        // Fuel tracking
        private float _lastFuelLevel = -1;
        private int _lastLap = -1;
        private float _fuelPerLap = 0;
        private float _fuelLapsRemaining = 0;

        public bool IsConnected => _sdk.IsConnected();
        public event EventHandler<TelemetryData>? OnTelemetryUpdated;
        public event EventHandler? OnConnected;
        public event EventHandler? OnDisconnected;

        public IRacingProvider()
        {
            _sdk = new IRacingSDK();
        }

        public Task StartAsync(CancellationToken cancellationToken = default)
        {
            _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            _wasConnected = false;

            // Start polling immediately - connection handling is inside the loop
            _pollingTask = PollingLoopAsync(_cts.Token);
            return Task.CompletedTask;
        }

        public async Task StopAsync()
        {
            _cts?.Cancel();
            
            if (_pollingTask != null)
            {
                try
                {
                    await _pollingTask;
                }
                catch (OperationCanceledException)
                {
                    // Esperado
                }
            }
        }

        private async Task PollingLoopAsync(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                try
                {
                    if (!_sdk.IsConnected())
                    {
                        if (_wasConnected)
                        {
                            _wasConnected = false;
                            OnDisconnected?.Invoke(this, EventArgs.Empty);
                        }

                        await Task.Delay(2000, ct);
                        continue;
                    }

                    if (!_wasConnected)
                    {
                        _wasConnected = true;
                        OnConnected?.Invoke(this, EventArgs.Empty);
                    }

                    var data = ReadTelemetryData();
                    TrackFuelConsumption(data);
                    OnTelemetryUpdated?.Invoke(this, data);

                    // 60 Hz
                    await Task.Delay(16, ct);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception)
                {
                    await Task.Delay(100, ct);
                }
            }
        }

        private void TrackFuelConsumption(TelemetryData data)
        {
            // Track fuel per lap when crossing lap boundary
            if (_lastLap > 0 && data.Lap > _lastLap && _lastFuelLevel > 0)
            {
                float fuelUsed = _lastFuelLevel - data.FuelLevel;
                if (fuelUsed > 0)
                {
                    // Smooth average with previous readings
                    _fuelPerLap = _fuelPerLap > 0
                        ? (_fuelPerLap * 0.7f + fuelUsed * 0.3f)
                        : fuelUsed;
                }
            }

            _lastLap = data.Lap;
            _lastFuelLevel = data.FuelLevel;

            // Calculate laps remaining
            if (_fuelPerLap > 0)
            {
                _fuelLapsRemaining = data.FuelLevel / _fuelPerLap;
            }
            else if (data.FuelUsePerHour > 0)
            {
                // Fallback: estimate from fuel use per hour (assuming ~90s laps)
                float fuelPerLapEstimate = data.FuelUsePerHour / 3600f * 90f;
                _fuelLapsRemaining = fuelPerLapEstimate > 0 ? data.FuelLevel / fuelPerLapEstimate : 0;
            }

            data.FuelPerLap = _fuelPerLap;
            data.FuelLapsRemaining = _fuelLapsRemaining;
        }

        private TelemetryData ReadTelemetryData()
        {
            return new TelemetryData
            {
                // Timing
                SessionTime = GetValue<float>("SessionTime"),
                Lap = GetValue<int>("Lap"),
                LapCurrentLapTime = GetValue<float>("LapCurrentLapTime"),
                LapLastLapTime = GetValue<float>("LapLastLapTime"),
                LapBestLapTime = GetValue<float>("LapBestLapTime"),
                LapDistPct = GetValue<float>("LapDistPct"),
                
                // Delta
                DeltaToBest = GetValue<float>("LapDeltaToBestLap"),
                IsDeltaValid = GetValue<bool>("LapDeltaToBestLap_OK"),
                
                // Car
                Speed = GetValue<float>("Speed") * 3.6f,  // m/s to km/h
                RPM = GetValue<float>("RPM"),
                Gear = GetValue<int>("Gear"),
                Throttle = GetValue<float>("Throttle") * 100f,
                Brake = GetValue<float>("Brake") * 100f,
                Clutch = GetValue<float>("Clutch") * 100f,
                SteeringWheelAngle = GetValue<float>("SteeringWheelAngle"),
                
                // Temps
                WaterTemp = GetValue<float>("WaterTemp"),
                OilTemp = GetValue<float>("OilTemp"),
                
                // Tire Temps
                TireTempLFInner = GetValue<float>("LFtempL"),
                TireTempLFMiddle = GetValue<float>("LFtempM"),
                TireTempLFOuter = GetValue<float>("LFtempR"),
                TireTempRFInner = GetValue<float>("RFtempL"),
                TireTempRFMiddle = GetValue<float>("RFtempM"),
                TireTempRFOuter = GetValue<float>("RFtempR"),
                TireTempLRInner = GetValue<float>("LRtempL"),
                TireTempLRMiddle = GetValue<float>("LRtempM"),
                TireTempLROuter = GetValue<float>("LRtempR"),
                TireTempRRInner = GetValue<float>("RRtempL"),
                TireTempRRMiddle = GetValue<float>("RRtempM"),
                TireTempRROuter = GetValue<float>("RRtempR"),
                
                // Fuel
                FuelLevel = GetValue<float>("FuelLevel"),
                FuelLevelPct = GetValue<float>("FuelLevelPct"),
                FuelUsePerHour = GetValue<float>("FuelUsePerHour"),
                
                // Position
                PositionX = GetValue<float>("PlayerCarPositionX"),
                PositionY = GetValue<float>("PlayerCarPositionY"),
                PositionZ = GetValue<float>("PlayerCarPositionZ"),
                
                // Session
                IsOnTrack = GetValue<bool>("IsOnTrack"),
                SessionState = GetValue<int>("SessionState")
            };
        }

        private T GetValue<T>(string varName)
        {
            try
            {
                var value = _sdk.GetData(varName);
                if (value == null) return default(T);
                
                // Conversão de tipos
                if (typeof(T) == typeof(float) && value is double d)
                    return (T)(object)(float)d;
                if (typeof(T) == typeof(float) && value is float f)
                    return (T)(object)f;
                if (typeof(T) == typeof(int) && value is int i)
                    return (T)(object)i;
                if (typeof(T) == typeof(bool) && value is bool b)
                    return (T)(object)b;
                
                return (T)value;
            }
            catch
            {
                return default(T);
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                StopAsync().GetAwaiter().GetResult();
                _cts?.Dispose();
            }
        }
    }

    public interface ITelemetryProvider
    {
        bool IsConnected { get; }
        event EventHandler<TelemetryData>? OnTelemetryUpdated;
        event EventHandler? OnConnected;
        event EventHandler? OnDisconnected;
        
        Task StartAsync(CancellationToken cancellationToken = default);
        Task StopAsync();
    }
}
