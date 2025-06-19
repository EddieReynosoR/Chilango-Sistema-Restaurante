using SistemaRestaurante.Models;
using SistemaRestaurante.Repositories;
using System.Collections.ObjectModel;
using System.Windows;

namespace SistemaRestaurante.ViewModels
{
    internal class InventarioViewModel : ViewModelBase
    {
        private ObservableCollection<Producto> _productos;
        public ObservableCollection<Producto> Productos
        {
            get => _productos;
            set
            {
                _productos = value;
                OnPropertyChanged(nameof(Productos));
            }
        }

        public int CantidadProductos => Productos?.Count ?? 0;

        private readonly ProductoRepository _productoRepository;

        public InventarioViewModel()
        {
            _productoRepository = new ProductoRepository(new SoftwareRestauranteContext());
            CargarProductos();
        }

        public async void CargarProductos()
        {
            Productos = new ObservableCollection<Producto>(await Task.Run(() => _productoRepository.ObtenerProductos()));

            OnPropertyChanged(nameof(CantidadProductos));
        }

        public bool EliminarProducto(Producto producto)
        {
            try
            {
                var result = MessageBox.Show($"¿Estás seguro de eliminar el producto '{producto.Nombre}'?",
                    "Confirmar eliminación", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result != MessageBoxResult.Yes)
                    return false;

                if (!_productoRepository.EliminarProducto(producto))
                {
                    MessageBox.Show("Ocurrió un error al eliminar el producto.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                    return false;
                }

                MessageBox.Show("Producto eliminado exitosamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch(Exception e)
            {
                MessageBox.Show($"Ocurrió un error al eliminar el producto. {e.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                return false;
            }

            return true;
        }
    }
}
