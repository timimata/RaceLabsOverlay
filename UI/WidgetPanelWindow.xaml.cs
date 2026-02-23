using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RaceLabsOverlay.UI
{
    public partial class WidgetPanelWindow : Window
    {
        private readonly WidgetManager _widgetManager;
        private readonly OverlayWindow _overlayWindow;
        private readonly Dictionary<string, CheckBox> _checkboxes = new();

        public WidgetPanelWindow(WidgetManager widgetManager, OverlayWindow overlayWindow)
        {
            InitializeComponent();
            _widgetManager = widgetManager;
            _overlayWindow = overlayWindow;

            BuildWidgetList();

            _widgetManager.OnWidgetVisibilityChanged += OnWidgetVisibilityChanged;
        }

        private void BuildWidgetList()
        {
            WidgetList.Children.Clear();
            _checkboxes.Clear();

            foreach (var name in _widgetManager.GetAllWidgetNames())
            {
                var cb = new CheckBox
                {
                    Content = name,
                    IsChecked = _widgetManager.IsWidgetVisible(name),
                    Foreground = Brushes.White,
                    FontSize = 14,
                    Margin = new Thickness(0, 4, 0, 4),
                    Cursor = System.Windows.Input.Cursors.Hand,
                    Tag = name
                };
                cb.Checked += OnWidgetChecked;
                cb.Unchecked += OnWidgetUnchecked;

                _checkboxes[name] = cb;
                WidgetList.Children.Add(cb);
            }
        }

        private void OnWidgetChecked(object sender, RoutedEventArgs e)
        {
            var name = (string)((CheckBox)sender).Tag;
            _widgetManager.ShowWidget(name);
        }

        private void OnWidgetUnchecked(object sender, RoutedEventArgs e)
        {
            var name = (string)((CheckBox)sender).Tag;
            _widgetManager.HideWidget(name);
        }

        private void OnWidgetVisibilityChanged(string name, bool visible)
        {
            if (_checkboxes.TryGetValue(name, out var cb))
                cb.IsChecked = visible;
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            _widgetManager.ResetLayout();
        }

        private void EditModeButton_Click(object sender, RoutedEventArgs e)
        {
            _overlayWindow.ToggleEditModePublic();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            // Hide instead of close so it can be reopened
            e.Cancel = true;
            Hide();
        }
    }
}
