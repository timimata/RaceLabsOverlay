using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace RaceLabsOverlay.Services
{
    /// <summary>
    /// Serviço para gravar voltas de referência e fazer ghost racing.
    /// </summary>
    public class GhostRecorder
    {
        private readonly string _ghostsDirectory;
        private List<GhostFrame>? _currentRecording;
        private GhostLap? _bestLap;
        private bool _isRecording = false;
        private int _currentLapNumber = 0;
        
        public bool IsRecording => _isRecording;
        public GhostLap? BestLap => _bestLap;
        
        public event EventHandler<GhostLap>? OnBestLapImproved;
        public event EventHandler? OnRecordingStarted;
        public event EventHandler? OnRecordingStopped;

        public GhostRecorder(string? ghostsDirectory = null)
        {
            _ghostsDirectory = ghostsDirectory ?? Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "RaceLabsOverlay", "Ghosts");
            
            Directory.CreateDirectory(_ghostsDirectory);
        }

        public void StartRecording(int lapNumber)
        {
            _currentLapNumber = lapNumber;
            _currentRecording = new List<GhostFrame>();
            _isRecording = true;
            OnRecordingStarted?.Invoke(this, EventArgs.Empty);
        }

        public void RecordFrame(TelemetryData data)
        {
            if (!_isRecording || _currentRecording == null) return;
            
            // Gravar a 10 Hz é suficiente para ghost
            if (_currentRecording.Count == 0 || 
                (DateTime.Now - _currentRecording.Last().Timestamp).TotalMilliseconds >= 100)
            {
                _currentRecording.Add(new GhostFrame
                {
                    Timestamp = DateTime.Now,
                    LapDistPct = data.LapDistPct,
                    Speed = data.Speed,
                    LapTime = data.LapTime,
                    PositionX = data.PositionX,
                    PositionY = data.PositionY,
                    PositionZ = data.PositionZ
                });
            }
        }

        public async Task StopRecordingAsync(float lapTime, bool isValid)
        {
            if (!_isRecording) return;
            
            _isRecording = false;
            OnRecordingStopped?.Invoke(this, EventArgs.Empty);
            
            if (!isValid || _currentRecording == null || _currentRecording.Count < 10)
            {
                _currentRecording = null;
                return;
            }
            
            // Criar ghost lap
            var ghostLap = new GhostLap
            {
                TrackName = "Unknown",  // TODO: Obter da sessão
                CarName = "Unknown",
                LapTime = lapTime,
                DateRecorded = DateTime.Now,
                Frames = _currentRecording.ToList()
            };
            
            // Verificar se é a melhor volta
            if (_bestLap == null || lapTime < _bestLap.LapTime)
            {
                _bestLap = ghostLap;
                await SaveGhostAsync(ghostLap);
                OnBestLapImproved?.Invoke(this, ghostLap);
            }
            
            _currentRecording = null;
        }

        public GhostComparisonResult CompareWithGhost(float currentLapDistPct, float currentLapTime)
        {
            if (_bestLap == null || _bestLap.Frames.Count == 0)
            {
                return new GhostComparisonResult { HasGhost = false };
            }
            
            // Encontrar frame do ghost na mesma posição
            var ghostFrame = FindGhostFrameAtPosition(currentLapDistPct);
            if (ghostFrame == null)
            {
                return new GhostComparisonResult { HasGhost = false };
            }
            
            // Calcular delta
            float ghostTimeAtPos = ghostFrame.LapTime;
            float delta = currentLapTime - ghostTimeAtPos;
            
            // Calcular distância física (aproximada)
            // TODO: Calcular distância 3D real entre posições
            float distanceAhead = 0;  // Positivo = à frente do ghost
            
            return new GhostComparisonResult
            {
                HasGhost = true,
                DeltaSeconds = delta,
                IsAhead = delta < 0,
                GhostSpeed = ghostFrame.Speed,
                DistanceMeters = distanceAhead,
                TimeToFinish = _bestLap.LapTime - currentLapTime
            };
        }

        private GhostFrame? FindGhostFrameAtPosition(float lapDistPct)
        {
            if (_bestLap == null) return null;
            
            // Binary search ou linear search simples
            // Para simplicidade, linear com tolerância
            const float tolerance = 0.001f;  // 0.1% da pista
            
            return _bestLap.Frames.FirstOrDefault(f => 
                Math.Abs(f.LapDistPct - lapDistPct) < tolerance);
        }

        public async Task SaveGhostAsync(GhostLap ghost)
        {
            var fileName = $"{ghost.TrackName}_{ghost.CarName}_{ghost.DateRecorded:yyyyMMdd_HHmmss}.ghost";
            var filePath = Path.Combine(_ghostsDirectory, fileName);
            
            var json = JsonSerializer.Serialize(ghost, new JsonSerializerOptions
            {
                WriteIndented = false  // Compacto
            });
            
            await File.WriteAllTextAsync(filePath, json);
        }

        public async Task<GhostLap?> LoadGhostAsync(string filePath)
        {
            if (!File.Exists(filePath)) return null;
            
            var json = await File.ReadAllTextAsync(filePath);
            return JsonSerializer.Deserialize<GhostLap>(json);
        }

        public List<GhostLapInfo> GetAvailableGhosts()
        {
            var ghosts = new List<GhostLapInfo>();
            
            foreach (var file in Directory.GetFiles(_ghostsDirectory, "*.ghost"))
            {
                try
                {
                    var json = File.ReadAllText(file);
                    var ghost = JsonSerializer.Deserialize<GhostLap>(json);
                    if (ghost != null)
                    {
                        ghosts.Add(new GhostLapInfo
                        {
                            FilePath = file,
                            TrackName = ghost.TrackName,
                            CarName = ghost.CarName,
                            LapTime = ghost.LapTime,
                            DateRecorded = ghost.DateRecorded
                        });
                    }
                }
                catch { /* Ignorar ficheiros corrompidos */ }
            }
            
            return ghosts.OrderBy(g => g.LapTime).ToList();
        }

        public void SetBestLap(GhostLap lap)
        {
            _bestLap = lap;
        }
    }

    public class GhostFrame
    {
        public DateTime Timestamp { get; set; }
        public float LapDistPct { get; set; }  // 0.0 a 1.0
        public float Speed { get; set; }
        public float LapTime { get; set; }     // Tempo acumulado da volta
        public double PositionX { get; set; }
        public double PositionY { get; set; }
        public double PositionZ { get; set; }
    }

    public class GhostLap
    {
        public string TrackName { get; set; } = "";
        public string CarName { get; set; } = "";
        public float LapTime { get; set; }
        public DateTime DateRecorded { get; set; }
        public List<GhostFrame> Frames { get; set; } = new();
    }

    public class GhostLapInfo
    {
        public string FilePath { get; set; } = "";
        public string TrackName { get; set; } = "";
        public string CarName { get; set; } = "";
        public float LapTime { get; set; }
        public DateTime DateRecorded { get; set; }
        public string LapTimeFormatted => TimeSpan.FromSeconds(LapTime).ToString("m\\:ss\\.fff");
    }

    public class GhostComparisonResult
    {
        public bool HasGhost { get; set; }
        public float DeltaSeconds { get; set; }
        public bool IsAhead => DeltaSeconds < 0;
        public string DeltaFormatted => $"{(DeltaSeconds >= 0 ? "+" : "")}{DeltaSeconds:F2}";
        public float GhostSpeed { get; set; }
        public float DistanceMeters { get; set; }
        public float TimeToFinish { get; set; }
    }
}
