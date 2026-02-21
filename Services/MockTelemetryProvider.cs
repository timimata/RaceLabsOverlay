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
                Gear = _random.Next(1, 7)
            };
        }
    }
}