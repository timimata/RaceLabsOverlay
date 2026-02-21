using System;

namespace RaceLabsOverlay.Services
{
    public class MockTelemetryProvider : ITelemetryProvider
    {
        private readonly Random _random = new();
        
        public bool IsConnected => true;
        
        public TelemetryData GetData()
        {
            return new TelemetryData
            {
                Speed = _random.Next(0, 300),
                RPM = _random.Next(1000, 8000),
                Gear = _random.Next(1, 7),
                
                // Lap timing
                Lap = _random.Next(1, 20),
                LapCurrentLapTime = _random.Next(80, 120),
                LapLastLapTime = _random.Next(80, 120),
                LapBestLapTime = _random.Next(75, 100),
                LapDistPct = (float)_random.NextDouble(),
                
                // Fuel
                FuelLevel = (float)(_random.NextDouble() * 60),
                FuelLapsRemaining = (float)(_random.NextDouble() * 20),
                FuelPerLap = (float)(_random.NextDouble() * 5),
                
                // Inputs
                Throttle = (float)_random.NextDouble(),
                Brake = (float)_random.NextDouble(),
                Clutch = 0,
                
                // Tire temps
                TireTempLF = _random.Next(60, 110),
                TireTempRF = _random.Next(60, 110),
                TireTempLR = _random.Next(60, 110),
                TireTempRR = _random.Next(60, 110)
            };
        }
    }
}