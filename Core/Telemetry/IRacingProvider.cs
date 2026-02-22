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

        public bool IsConnected => _sdk.IsConnected();
        public event EventHandler<TelemetryData>? OnTelemetryUpdated;
        public event EventHandler? OnConnected;
        public event EventHandler? OnDisconnected;

        public IRacingProvider()
        {
            _sdk = new IRacingSDK();
        }

        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            
            // Tentar conectar
            while (!_cts.Token.IsCancellationRequested)
            {
                if (_sdk.IsConnected())
                {
                    OnConnected?.Invoke(this, EventArgs.Empty);
                    break;
                }
                
                await Task.Delay(1000, _cts.Token);
            }
            
            // Iniciar polling
            _pollingTask = PollingLoopAsync(_cts.Token);
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
                        OnDisconnected?.Invoke(this, EventArgs.Empty);
                        
                        // Tentar reconectar
                        await Task.Delay(2000, ct);
                        continue;
                    }
                    
                    var data = ReadTelemetryData();
                    OnTelemetryUpdated?.Invoke(this, data);
                    
                    // 60 Hz
                    await Task.Delay(16, ct);
                }
                catch (Exception ex)
                {
                    // Log error
                    await Task.Delay(100, ct);
                }
            }
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
