using SistemaRestaurante.Repositories;
using SistemaRestaurante.Models;
using System.Collections.ObjectModel;
using System.Windows;

namespace SistemaRestaurante.ViewModels
{
    class AgregarIngredientePlatilloViewModel : ViewModelBase
    {
        private ObservableCollection<ProductoSeleccionado> _productos;
        private ObservableCollection<Producto> _listaProductosSeleccionados;
        private bool _areAllSelected;

        public ObservableCollection<ProductoSeleccionado> Productos
        {
            get => _productos;
            set
            {
                _productos = value;
                OnPropertyChanged(nameof(Productos));
            }
        }

        public ObservableCollection<Producto> ListaProductosSeleccionados
        {
            get => _listaProductosSeleccionados;
            set
            {
                _listaProductosSeleccionados = value;
                OnPropertyChanged(nameof(Productos));
            }
        }

        public bool AreAllSelected
        {
            get => _areAllSelected;
            set
            {
                if (_areAllSelected != value)
                {
                    _areAllSelected = value;
                    OnPropertyChanged(nameof(AreAllSelected));

                    if (Productos != null)
                    {
                        foreach (var p in Productos)
                        {
                            p.IsSelected = _areAllSelected;
                        }
                    }
                }
            }
        }

        private readonly ProductoRepository _productoRepository;

        public int CantidadProductos => Productos?.Count ?? 0;

        public AgregarIngredientePlatilloViewModel(List<Producto>? productos = null)
        {
            _productoRepository = new ProductoRepository(new SoftwareRestauranteContext());
            CargarProductos(productos);
        }

        public List<Producto>? ProductosSeleccionados => Productos?.Where(p => p.IsSelected).Select(p => p.Producto).ToList();

        public async void CargarProductos(List<Producto>? productos = null)
        {
            var lista = await Task.Run(() => _productoRepository.ObtenerProductos());

            if (productos == null)
            {
                
                Productos = [.. lista.Select(p => new ProductoSeleccionado(p))];
                OnPropertyChanged(nameof(CantidadProductos));
                return;
            }

            Productos = [.. lista.Select(p =>
            {
                var seleccionado = productos.Any(x => x.IdProducto == p.IdProducto);
                return new ProductoSeleccionado(p) { IsSelected = seleccionado };
            })];

            OnPropertyChanged(nameof(CantidadProductos));
        }

        public bool GuardarProductos()
        {
            try
            {
                var productos = ProductosSeleccionados;

                if (productos?.Any() != true)
                {
                    MessageBox.Show("No se seleccionaron ingredientes para el platillo.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                    return false;
                }

                productos.ForEach(p => p.StockActual = 1);

                ListaProductosSeleccionados = [.. productos];
                MessageBox.Show("Se agregaron los productos correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception e)
            {
                MessageBox.Show($"Ocurrió un error al asignar los productos al platillo. {e.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                return false;
            }

            return true;
        }
    }

    class ProductoSeleccionado(Producto producto) : ViewModelBase
    {
        public Producto Producto { get; } = producto;

        private bool _isSelected;

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                }
            }
        }
    }
}
