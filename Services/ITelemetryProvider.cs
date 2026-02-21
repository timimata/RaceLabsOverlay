namespace RaceLabsOverlay.Services
{
    public interface ITelemetryProvider
    {
        bool IsConnected { get; }
        TelemetryData GetData();
    }

    public class TelemetryData
    {
        public double Speed { get; set; }
        public double RPM { get; set; }
        public int Gear { get; set; }
        
        // Lap timing
        public int Lap { get; set; }
        public float LapCurrentLapTime { get; set; }
        public float LapLastLapTime { get; set; }
        public float LapBestLapTime { get; set; }
        public float LapDistPct { get; set; }  // % da volta completa
        
        // Fuel
        public float FuelLevel { get; set; }
        public float FuelLapsRemaining { get; set; }
        public float FuelPerLap { get; set; }
        
        // Inputs
        public float Throttle { get; set; }
        public float Brake { get; set; }
        public float Steering { get; set; }
        public float Clutch { get; set; }
        
        // Tire temps
        public float TireTempLF { get; set; }
        public float TireTempRF { get; set; }
        public float TireTempLR { get; set; }
        public float TireTempRR { get; set; }
    }
}