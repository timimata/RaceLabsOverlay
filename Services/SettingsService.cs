using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace RaceLabsOverlay.Services
{
    /// <summary>
    /// Gestão profissional de configurações.
    /// </summary>
    public class SettingsService
    {
        private readonly string _settingsPath;
        private AppSettings _settings;
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public SettingsService()
        {
            var appDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "RaceLabsOverlay");
            
            Directory.CreateDirectory(appDataPath);
            _settingsPath = Path.Combine(appDataPath, "settings.json");
            
            Load();
        }

        public AppSettings Settings => _settings;

        public void Load()
        {
            try
            {
                if (File.Exists(_settingsPath))
                {
                    var json = File.ReadAllText(_settingsPath);
                    _settings = JsonSerializer.Deserialize<AppSettings>(json, _jsonOptions) ?? new AppSettings();
                }
                else
                {
                    _settings = new AppSettings();
                    Save();
                }
            }
            catch (Exception ex)
            {
                LoggingService.LogError(ex, "Failed to load settings");
                _settings = new AppSettings();
            }
        }

        public void Save()
        {
            try
            {
                var json = JsonSerializer.Serialize(_settings, _jsonOptions);
                File.WriteAllText(_settingsPath, json);
            }
            catch (Exception ex)
            {
                LoggingService.LogError(ex, "Failed to save settings");
            }
        }

        public T Get<T>(string key, T defaultValue)
        {
            if (_settings.CustomSettings.TryGetValue(key, out var value))
            {
                try
                {
                    return (T)Convert.ChangeType(value, typeof(T));
                }
                catch { }
            }
            return defaultValue;
        }

        public void Set<T>(string key, T value)
        {
            _settings.CustomSettings[key] = value?.ToString() ?? "";
            Save();
        }
    }

    public class AppSettings
    {
        // General
        public bool StartWithWindows { get; set; } = false;
        public bool CheckForUpdatesOnStartup { get; set; } = true;
        public bool MinimizeToTray { get; set; } = true;
        public string Language { get; set; } = "en";
        
        // Overlay
        public double OverlayOpacity { get; set; } = 0.95;
        public bool ClickThroughByDefault { get; set; } = true;
        public bool ShowInTaskbar { get; set; } = false;
        
        // Widgets
        public bool EnableGhostRecording { get; set; } = true;
        public bool AutoSaveGhostOnBestLap { get; set; } = true;
        public int GhostRecordingQuality { get; set; } = 10; // Hz
        
        // Update
        public string? SkippedVersion { get; set; }
        public string UpdateChannel { get; set; } = "stable"; // stable, beta, dev
        
        // Hotkeys
        public string ToggleEditModeHotkey { get; set; } = "Ctrl+Shift+O";
        public string ToggleClickThroughHotkey { get; set; } = "Ctrl+Shift+H";
        public string RecordGhostHotkey { get; set; } = "Ctrl+Shift+R";
        
        // Window Position
        public double WindowLeft { get; set; } = 0;
        public double WindowTop { get; set; } = 0;
        public double WindowWidth { get; set; } = 1920;
        public double WindowHeight { get; set; } = 1080;
        
        // Widget Layouts
        public List<WidgetLayoutEntry> WidgetLayouts { get; set; } = new();

        // Custom
        public Dictionary<string, string> CustomSettings { get; set; } = new();
    }

    public class WidgetLayoutEntry
    {
        public string WidgetName { get; set; } = "";
        public bool IsVisible { get; set; } = true;
        public double X { get; set; }
        public double Y { get; set; }
    }
}
