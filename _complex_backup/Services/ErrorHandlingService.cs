using System;
using System.Windows;
using System.Windows.Threading;

namespace RaceLabsOverlay.Services
{
    /// <summary>
    /// Gestão profissional de erros e crashes.
    /// </summary>
    public static class ErrorHandlingService
    {
        public static void Initialize()
        {
            // Global exception handlers
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            Application.Current.DispatcherUnhandledException += OnDispatcherUnhandledException;
            TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
        }

        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = e.ExceptionObject as Exception;
            HandleFatalError(exception, "Unhandled AppDomain Exception");
        }

        private static void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            LoggingService.LogError(e.Exception, "Unhandled Dispatcher Exception");
            
            // Show user-friendly error
            var result = MessageBox.Show(
                $"An unexpected error occurred:\n\n{e.Exception.Message}\n\nDo you want to continue?",
                "Error",
                MessageBoxButton.YesNo,
                MessageBoxImage.Error);
            
            if (result == MessageBoxResult.Yes)
            {
                e.Handled = true;
            }
            else
            {
                Application.Current.Shutdown(1);
            }
        }

        private static void OnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
        {
            LoggingService.LogError(e.Exception, "Unobserved Task Exception");
            e.SetObserved();
        }

        private static void HandleFatalError(Exception? ex, string context)
        {
            var message = ex?.ToString() ?? "Unknown error";
            
            LoggingService.LogFatal(ex, context);
            
            // Save crash report
            try
            {
                var crashReport = $@"
Crash Report - {DateTime.Now:yyyy-MM-dd HH:mm:ss}
================================================
Version: {AppVersion.Current}
OS: {Environment.OSVersion}
Machine: {Environment.MachineName}
Context: {context}

Exception:
{message}

Stack Trace:
{ex?.StackTrace}
";
                var crashPath = System.IO.Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "RaceLabsOverlay", "Crashes", $"crash_{DateTime.Now:yyyyMMdd_HHmmss}.txt");
                
                System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(crashPath)!);
                System.IO.File.WriteAllText(crashPath, crashReport);
            }
            catch { }
            
            // Show fatal error dialog
            MessageBox.Show(
                $"A fatal error has occurred:\n\n{ex?.Message}\n\nThe application will now close.\n\nA crash report has been saved.",
                "Fatal Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            
            Environment.Exit(1);
        }
    }
}
