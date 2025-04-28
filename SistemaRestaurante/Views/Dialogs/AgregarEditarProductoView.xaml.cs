using SistemaRestaurante.Models;
using SistemaRestaurante.ViewModels;
using System.Windows;

namespace SistemaRestaurante.Views.Dialogs
{
    /// <summary>
    /// Interaction logic for AgregarEditarProductoView.xaml
    /// </summary>
    public partial class AgregarEditarProductoView : Window
    {
        private readonly AgregarEditarProductoViewModel _viewModel;

        public AgregarEditarProductoView(Producto? producto = null)
        {
            InitializeComponent();
            _viewModel = new AgregarEditarProductoViewModel(producto);
            DataContext = _viewModel;
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.ModalTitle = "Agregar Producto";
            if (_viewModel.GuardarProducto())
                Close();
        }
    }
}
