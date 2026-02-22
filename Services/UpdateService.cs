using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace RaceLabsOverlay.Services
{
    /// <summary>
    /// Sistema profissional de auto-update.
    /// </summary>
    public class UpdateService
    {
        private readonly HttpClient _httpClient;
        private readonly string _currentVersion;
        private readonly string _updateUrl;
        private readonly string _tempPath;
        
        public event EventHandler<UpdateAvailableEventArgs>? OnUpdateAvailable;
        public event EventHandler<UpdateProgressEventArgs>? OnUpdateProgress;
        public event EventHandler? OnUpdateCompleted;
        public event EventHandler<string>? OnUpdateError;

        public UpdateService(string currentVersion, string updateUrl)
        {
            _currentVersion = currentVersion;
            _updateUrl = updateUrl;
            _httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(30) };
            _tempPath = Path.Combine(Path.GetTempPath(), "RaceLabsOverlay_Updates");
        }

        /// <summary>
        /// Verifica se existe uma nova versão.
        /// </summary>
        public async Task<UpdateInfo?> CheckForUpdateAsync()
        {
            try
            {
                var response = await _httpClient.GetStringAsync($"{_updateUrl}/version.json");
                var updateInfo = JsonSerializer.Deserialize<UpdateInfo>(response);
                
                if (updateInfo == null) return null;
                
                var current = new Version(_currentVersion);
                var latest = new Version(updateInfo.Version);
                
                if (latest > current)
                {
                    return updateInfo;
                }
                
                return null;
            }
            catch (Exception ex)
            {
                OnUpdateError?.Invoke(this, $"Failed to check for updates: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Download e instalação do update.
        /// </summary>
        public async Task DownloadAndInstallUpdateAsync(UpdateInfo updateInfo, IProgress<UpdateProgress>? progress = null)
        {
            try
            {
                Directory.CreateDirectory(_tempPath);
                var zipPath = Path.Combine(_tempPath, $"update_{updateInfo.Version}.zip");
                
                // Download
                using (var response = await _httpClient.GetAsync(updateInfo.DownloadUrl, HttpCompletionOption.ResponseHeadersRead))
                {
                    response.EnsureSuccessStatusCode();
                    
                    var totalBytes = response.Content.Headers.ContentLength ?? -1L;
                    using var contentStream = await response.Content.ReadAsStreamAsync();
                    using var fileStream = new FileStream(zipPath, FileMode.Create, FileAccess.Write, FileShare.None);
                    
                    var buffer = new byte[8192];
                    long totalRead = 0;
                    int read;
                    
                    while ((read = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                    {
                        await fileStream.WriteAsync(buffer, 0, read);
                        totalRead += read;
                        
                        if (totalBytes > 0)
                        {
                            var percent = (int)((totalRead * 100) / totalBytes);
                            progress?.Report(new UpdateProgress(percent, $"Downloading... {percent}%"));
                        }
                    }
                }
                
                progress?.Report(new UpdateProgress(100, "Extracting..."));
                
                // Extract
                var extractPath = Path.Combine(_tempPath, $"extract_{updateInfo.Version}");
                Directory.CreateDirectory(extractPath);
                ZipFile.ExtractToDirectory(zipPath, extractPath, true);
                
                progress?.Report(new UpdateProgress(100, "Installing..."));
                
                // Create updater script
                var appPath = AppDomain.CurrentDomain.BaseDirectory;
                var updaterScript = Path.Combine(_tempPath, "update.bat");
                
                var batchContent = $@"
@echo off
timeout /t 2 /nobreak > nul
xcopy ""{extractPath}\*"" ""{appPath}"" /E /Y /Q
rmdir /S /Q ""{extractPath}""
del ""{zipPath}""
start """"{Path.Combine(appPath, "RaceLabsOverlay.exe")}""
del ""%~f0""
";
                
                await File.WriteAllTextAsync(updaterScript, batchContent);
                
                progress?.Report(new UpdateProgress(100, "Restarting..."));
                
                // Launch updater and exit
                var psi = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/c \"{updaterScript}\"",
                    CreateNoWindow = true,
                    UseShellExecute = false
                };
                
                Process.Start(psi);
                
                OnUpdateCompleted?.Invoke(this, EventArgs.Empty);
                
                // Exit current app
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Application.Current.Shutdown();
                });
            }
            catch (Exception ex)
            {
                OnUpdateError?.Invoke(this, $"Update failed: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Verifica updates automaticamente no startup.
        /// </summary>
        public async Task CheckOnStartupAsync(bool silent = true)
        {
            var update = await CheckForUpdateAsync();
            
            if (update != null)
            {
                OnUpdateAvailable?.Invoke(this, new UpdateAvailableEventArgs(update));
            }
            else if (!silent)
            {
                // Show "no updates" message
                MessageBox.Show(
                    "You are running the latest version!",
                    "No Updates Available",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
        }
    }

    public class UpdateInfo
    {
        public string Version { get; set; } = "";
        public string DownloadUrl { get; set; } = "";
        public string ReleaseNotes { get; set; } = "";
        public DateTime ReleaseDate { get; set; }
        public bool IsMandatory { get; set; } = false;
        public long FileSize { get; set; }
    }

    public class UpdateAvailableEventArgs : EventArgs
    {
        public UpdateInfo UpdateInfo { get; }
        
        public UpdateAvailableEventArgs(UpdateInfo info)
        {
            UpdateInfo = info;
        }
    }

    public class UpdateProgress
    {
        public int Percentage { get; }
        public string Status { get; }
        
        public UpdateProgress(int percentage, string status)
        {
            Percentage = percentage;
            Status = status;
        }
    }

    public class UpdateProgressEventArgs : EventArgs
    {
        public UpdateProgress Progress { get; }

        public UpdateProgressEventArgs(UpdateProgress progress)
        {
            Progress = progress;
        }
    }
}
