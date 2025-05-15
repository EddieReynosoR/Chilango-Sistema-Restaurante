using SistemaRestaurante.Models;
using SistemaRestaurante.ViewModels;
using SistemaRestaurante.Views.Dialogs;
using System.Windows;
using System.Windows.Controls;

namespace SistemaRestaurante.Views.Pages
{
    /// <summary>
    /// Interaction logic for CortePage.xaml
    /// </summary>
    public partial class PlatillosPage : Page
    {
        private readonly PlatilloViewModel _viewModel;

        public PlatillosPage()
        {
            InitializeComponent();

            _viewModel = new PlatilloViewModel();
            DataContext = _viewModel;
        }

        private void btnAñadirPlatillo_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var modal = new AgregarEditarPlatilloWindow();
            modal.ShowDialog();

            _viewModel.CargarPlatillos();
        }

        private void btnEditar_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.Tag is Platillo platillo)
            {
                var modal = new AgregarEditarPlatilloWindow(platillo);
                modal.ShowDialog();

                _viewModel.CargarPlatillos();
            }
        }

        private void btnEliminar_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.Tag is Platillo platillo)
            {
                if(_viewModel.EliminarPlatillo(platillo))
                {
                    MessageBox.Show("Ocurrió un error al abrir la orden.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                _viewModel.CargarPlatillos();
            }
        }
    }
}
