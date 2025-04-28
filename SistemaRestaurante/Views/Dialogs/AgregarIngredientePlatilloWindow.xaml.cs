using SistemaRestaurante.Models;
using SistemaRestaurante.ViewModels;
using System.Collections.ObjectModel;
using System.Windows;

namespace SistemaRestaurante.Views.Dialogs
{
    /// <summary>
    /// Interaction logic for AgregarIngredientePlatilloWindow.xaml
    /// </summary>
    public partial class AgregarIngredientePlatilloWindow : Window
    {
        private readonly AgregarIngredientePlatilloViewModel _viewModel;

        public ObservableCollection<Producto> ProductosSeleccionadosResultado { get; private set; }

        public AgregarIngredientePlatilloWindow(List<Producto>? productos = null)
        {
            InitializeComponent();

            _viewModel = new AgregarIngredientePlatilloViewModel(productos);
            DataContext = _viewModel;
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.GuardarProductos();

            ProductosSeleccionadosResultado = _viewModel.ListaProductosSeleccionados;
            DialogResult = true;
            Close();
        }
    }
}
