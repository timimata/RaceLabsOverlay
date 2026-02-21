using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace RaceLabsOverlay.UI
{
    /// <summary>
    /// Sistema de notificações flutuantes no overlay.
    /// </summary>
    public class NotificationManager
    {
        private readonly Canvas _canvas;
        private readonly StackPanel _notificationPanel;
        
        public NotificationManager(Canvas canvas)
        {
            _canvas = canvas;
            _notificationPanel = new StackPanel
            {
                Orientation = Orientation.Vertical,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(0, 80, 20, 0)
            };
            
            Canvas.SetRight(_notificationPanel, 20);
            Canvas.SetTop(_notificationPanel, 80);
            _canvas.Children.Add(_notificationPanel);
        }
        
        public void ShowNotification(string message, NotificationType type = NotificationType.Info, int durationMs = 3000)
        {
            var border = new Border
            {
                Background = GetBackgroundColor(type),
                BorderBrush = GetBorderColor(type),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(15, 10, 15, 10),
                Margin = new Thickness(0, 0, 0, 10),
                Opacity = 0,
                RenderTransform = new TranslateTransform(50, 0)
            };
            
            var textBlock = new TextBlock
            {
                Text = message,
                Foreground = Brushes.White,
                FontSize = 13,
                FontWeight = FontWeights.Medium,
                MaxWidth = 300,
                TextWrapping = TextWrapping.Wrap
            };
            
            border.Child = textBlock;
            _notificationPanel.Children.Add(border);
            
            // Animação de entrada
            var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(300));
            var slideIn = new DoubleAnimation(50, 0, TimeSpan.FromMilliseconds(300));
            
            border.BeginAnimation(UIElement.OpacityProperty, fadeIn);
            (border.RenderTransform as TranslateTransform)?.BeginAnimation(TranslateTransform.XProperty, slideIn);
            
            // Remover após duração
            var timer = new System.Windows.Threading.DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(durationMs)
            };
            
            timer.Tick += (s, e) =>
            {
                timer.Stop();
                RemoveNotification(border);
            };
            
            timer.Start();
        }
        
        private void RemoveNotification(Border border)
        {
            // Animação de saída
            var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(300));
            var slideOut = new DoubleAnimation(0, 50, TimeSpan.FromMilliseconds(300));
            
            fadeOut.Completed += (s, e) =>
            {
                _notificationPanel.Children.Remove(border);
            };
            
            border.BeginAnimation(UIElement.OpacityProperty, fadeOut);
            (border.RenderTransform as TranslateTransform)?.BeginAnimation(TranslateTransform.XProperty, slideOut);
        }
        
        private Brush GetBackgroundColor(NotificationType type) => type switch
        {
            NotificationType.Success => new SolidColorBrush(Color.FromArgb(230, 34, 136, 34)),
            NotificationType.Warning => new SolidColorBrush(Color.FromArgb(230, 255, 136, 0)),
            NotificationType.Error => new SolidColorBrush(Color.FromArgb(230, 204, 51, 51)),
            _ => new SolidColorBrush(Color.FromArgb(230, 51, 51, 51))
        };
        
        private Brush GetBorderColor(NotificationType type) => type switch
        {
            NotificationType.Success => new SolidColorBrush(Color.FromRgb(0, 255, 0)),
            NotificationType.Warning => new SolidColorBrush(Color.FromRgb(255, 200, 0)),
            NotificationType.Error => new SolidColorBrush(Color.FromRgb(255, 100, 100)),
            _ => new SolidColorBrush(Color.FromRgb(100, 100, 100))
        };
    }
    
    public enum NotificationType
    {
        Info,
        Success,
        Warning,
        Error
    }
}
