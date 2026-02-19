using System.Windows;

namespace RaceLabsOverlay
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            // Configurar logging
            ConfigureLogging();
            
            // Configurar DI container
            var services = ConfigureServices();
            
            // Iniciar aplicação
            var mainWindow = new UI.OverlayWindow();
            mainWindow.Show();
        }
        
        private void ConfigureLogging()
        {
            // TODO: Configurar Serilog
        }
        
        private IServiceProvider ConfigureServices()
        {
            // TODO: Configurar DI
            return null;
        }
    }
}
