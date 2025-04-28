using SistemaRestaurante.Utilities;
using SistemaRestaurante.ViewModels;
using SistemaRestaurante.Views;
using SistemaRestaurante.Views.Pages;
using System.Windows;
using System.Windows.Input;

namespace SistemaRestaurante
{
    public partial class MainWindow : Window
    {
        bool isMaximized = false;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();

            MainFrame.Navigate(new MainPage());

            NavBar.HomeClicked += () =>
            {
                MainFrame.Navigate(new MainPage());
            };

            NavBar.MesasClicked += () =>
            {
                MainFrame.Navigate(new MesasPage());
            };

            NavBar.VentasClicked += () =>
            {
                MainFrame.Navigate(new VentasPage());
            };

            NavBar.PlatillosClicked += () =>
            {
                MainFrame.Navigate(new PlatillosPage());
            };

            NavBar.InventarioClicked += () =>
            {
                MainFrame.Navigate(new InventarioPage());
            };

            NavBar.CerrarSesionClicked += CerrarSesion;
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                if (isMaximized)
                {
                    WindowState = WindowState.Normal;
                    Width = 1280;
                    Height = 780;

                    isMaximized = false;

                    return;
                }

                WindowState = WindowState.Maximized;
                isMaximized = true;
            }
        }

        private void CerrarSesion()
        {
            var result = MessageBox.Show("¿Seguro que quieres cerrar sesión?", "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                Session.Instance.Usuario = null;
                GlobalUtilities.OpenWindow(new LoginView(), this);
            }
        }
    }
}