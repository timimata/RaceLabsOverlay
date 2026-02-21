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
    }
}