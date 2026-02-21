using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace RaceLabsOverlay.Services
{
    /// <summary>
    /// Sistema de perfis para salvar/carregar layouts de widgets.
    /// </summary>
    public class ProfileManager
    {
        private readonly string _profilesPath;
        private List<WidgetProfile> _profiles = new();
        
        public ProfileManager()
        {
            _profilesPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "RaceLabsOverlay", "Profiles");
            
            Directory.CreateDirectory(_profilesPath);
            LoadProfiles();
        }
        
        public IReadOnlyList<WidgetProfile> Profiles => _profiles.AsReadOnly();
        
        public void SaveProfile(string name, List<WidgetPosition> widgetPositions)
        {
            var profile = new WidgetProfile
            {
                Name = name,
                CreatedAt = DateTime.Now,
                Widgets = widgetPositions
            };
            
            var existing = _profiles.FindIndex(p => p.Name == name);
            if (existing >= 0)
            {
                _profiles[existing] = profile;
            }
            else
            {
                _profiles.Add(profile);
            }
            
            SaveToFile(profile);
        }
        
        public WidgetProfile? LoadProfile(string name)
        {
            var filePath = Path.Combine(_profilesPath, $"{name}.json");
            if (!File.Exists(filePath)) return null;
            
            try
            {
                var json = File.ReadAllText(filePath);
                return JsonSerializer.Deserialize<WidgetProfile>(json);
            }
            catch
            {
                return null;
            }
        }
        
        public void DeleteProfile(string name)
        {
            _profiles.RemoveAll(p => p.Name == name);
            var filePath = Path.Combine(_profilesPath, $"{name}.json");
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
        
        private void LoadProfiles()
        {
            if (!Directory.Exists(_profilesPath)) return;
            
            foreach (var file in Directory.GetFiles(_profilesPath, "*.json"))
            {
                try
                {
                    var json = File.ReadAllText(file);
                    var profile = JsonSerializer.Deserialize<WidgetProfile>(json);
                    if (profile != null)
                    {
                        _profiles.Add(profile);
                    }
                }
                catch { /* ignorar ficheiros corrompidos */ }
            }
        }
        
        private void SaveToFile(WidgetProfile profile)
        {
            var filePath = Path.Combine(_profilesPath, $"{profile.Name}.json");
            var json = JsonSerializer.Serialize(profile, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, json);
        }
    }
    
    public class WidgetProfile
    {
        public string Name { get; set; } = "Default";
        public DateTime CreatedAt { get; set; }
        public List<WidgetPosition> Widgets { get; set; } = new();
    }
    
    public class WidgetPosition
    {
        public string WidgetType { get; set; } = "";
        public double X { get; set; }
        public double Y { get; set; }
        public bool IsVisible { get; set; } = true;
    }
}
