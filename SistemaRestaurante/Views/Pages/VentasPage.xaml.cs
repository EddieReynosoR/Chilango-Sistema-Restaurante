using SistemaRestaurante.Models;
using SistemaRestaurante.ViewModels;
using SistemaRestaurante.Views.Dialogs;
using System.Windows;
using System.Windows.Controls;

namespace SistemaRestaurante.Views.Pages
{
    /// <summary>
    /// Interaction logic for VentasPage.xaml
    /// </summary>
    public partial class VentasPage : Page
    {
        private readonly VentaViewModel _viewModel;

        public VentasPage()
        {
            InitializeComponent();
            _viewModel = new VentaViewModel();
            DataContext = _viewModel;
        }

        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            if (button?.Tag is Ventum venta && _viewModel.EliminarProducto(venta))
                _viewModel.CargarVentas();
        }

        private void btnCorteCaja_Click(object sender, RoutedEventArgs e)
        {
            var modal = new FechaDialog();
            modal.ViewModel = _viewModel;
            modal.Show();
        }
    }
}
