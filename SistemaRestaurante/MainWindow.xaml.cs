using SistemaRestaurante.Models;
using SistemaRestaurante.Utilities;
using SistemaRestaurante.Utilities.Facade;
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
        private readonly RestauranteFacade _restauranteFacade;

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

            _restauranteFacade = new RestauranteFacade(new SoftwareRestauranteContext());
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

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!_restauranteFacade.ExisteMesaActiva())
                return;

            MessageBox.Show("Hay mesas con órdenes activas, termínalas o cancelalas para salir de la aplicación.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            e.Cancel = true;
        }
    }
}