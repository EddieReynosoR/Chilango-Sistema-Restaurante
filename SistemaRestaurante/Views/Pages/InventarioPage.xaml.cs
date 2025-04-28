using SistemaRestaurante.Models;
using SistemaRestaurante.ViewModels;
using SistemaRestaurante.Views.Dialogs;
using System.Windows.Controls;

namespace SistemaRestaurante.Views.Pages
{
    /// <summary>
    /// Interaction logic for InventarioPage.xaml
    /// </summary>
    public partial class InventarioPage : Page
    {
        private readonly InventarioViewModel _viewModel;

        public InventarioPage()
        {
            InitializeComponent();

            _viewModel = new InventarioViewModel();
            DataContext = _viewModel;
        }

        private void btnAñadirProducto_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var modal = new AgregarEditarProductoView();
            modal.ShowDialog();

            _viewModel.CargarProductos();
        }

        private void btnEliminar_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var button = sender as Button;

            if (button?.Tag is Producto producto && _viewModel.EliminarProducto(producto))
                _viewModel.CargarProductos();
        }

        private void btnEditar_Click(object sender, System.Windows.RoutedEventArgs e)
        {          
            var button = sender as Button;
            if (button?.Tag is Producto producto)
            {
                var modal = new AgregarEditarProductoView(producto);
                modal.ShowDialog();

                _viewModel.CargarProductos();
            }
        }
    }
}
