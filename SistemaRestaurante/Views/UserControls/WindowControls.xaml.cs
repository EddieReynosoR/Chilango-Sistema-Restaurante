using System.Windows;
using System.Windows.Controls;

namespace SistemaRestaurante.Views.UserControls
{
    /// <summary>
    /// Interaction logic for WindowControls.xaml
    /// </summary>
    public partial class WindowControls : UserControl
    {
        public static readonly DependencyProperty ShowMinimizeProperty = DependencyProperty.Register("ShowMinimize", typeof(bool), typeof(WindowControls), new PropertyMetadata(true, OnVisibilityChanged));

        public static readonly DependencyProperty ShowMaxRestoreProperty =
        DependencyProperty.Register("ShowMaxRestore", typeof(bool), typeof(WindowControls), new PropertyMetadata(true, OnVisibilityChanged));

        public static readonly DependencyProperty ShowCloseProperty =
            DependencyProperty.Register("ShowClose", typeof(bool), typeof(WindowControls), new PropertyMetadata(true, OnVisibilityChanged));

        public bool ShowMinimize
        {
            get => (bool)GetValue(ShowMinimizeProperty);
            set => SetValue(ShowMinimizeProperty, value);
        }

        public bool ShowMaxRestore
        {
            get => (bool)GetValue(ShowMaxRestoreProperty);
            set => SetValue(ShowMaxRestoreProperty, value);
        }

        public bool ShowClose
        {
            get => (bool)GetValue(ShowCloseProperty);
            set => SetValue(ShowCloseProperty, value);
        }

        public WindowControls()
        {
            InitializeComponent();
            Loaded += (s, e) => UpdateButtonVisibility();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            Window window = Window.GetWindow(this);
            if (window != null)
                window.WindowState = WindowState.Minimized;
        }

        private void MaxRestoreButton_Click(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            if (window != null)
            {
                window.WindowState = window.WindowState == WindowState.Maximized
                    ? WindowState.Normal
                    : WindowState.Maximized;
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this)?.Close();
        }

        private static void OnVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (WindowControls)d;
            control.UpdateButtonVisibility();
        }

        private void UpdateButtonVisibility()
        {
            MinimizeButton.Visibility = ShowMinimize ? Visibility.Visible : Visibility.Collapsed;
            MaxRestoreButton.Visibility = ShowMaxRestore ? Visibility.Visible : Visibility.Collapsed;
            CloseButton.Visibility = ShowClose ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
