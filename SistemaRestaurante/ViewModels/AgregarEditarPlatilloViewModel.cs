using SistemaRestaurante.Models;
using SistemaRestaurante.Repositories;
using System.Collections.ObjectModel;
using System.Windows;

namespace SistemaRestaurante.ViewModels
{
    class AgregarEditarPlatilloViewModel : ViewModelBase
    {
        private readonly PlatilloRepository _platilloRepository;
        private ObservableCollection<ProductoViewModel> _productos;
        private string _modalTitle;
        private int _idPlatillo;
        private string _nombre;
        private decimal _precio;

        public ObservableCollection<ProductoViewModel> Productos
        {
            get => _productos;
            set
            {
                _productos = value;
                OnPropertyChanged();
            }
        }

        public int IdPlatillo
        {
            get => _idPlatillo;
            set
            {
                _idPlatillo = value;
                OnPropertyChanged(nameof(IdPlatillo));
            }
        }

        public string Nombre
        {
            get => _nombre;
            set
            {
                _nombre = value;
                OnPropertyChanged(nameof(Nombre));
            }
        }

        public decimal Precio
        {
            get => _precio;
            set
            {
                _precio = value;
                OnPropertyChanged(nameof(Precio));
            }
        }

        public string ModalTitle
        {
            get => _modalTitle;
            set
            {
                _modalTitle = value;
                OnPropertyChanged(nameof(ModalTitle));
            }
        }

        public AgregarEditarPlatilloViewModel(Platillo? platillo = null)
        {
            _platilloRepository = new PlatilloRepository(new SoftwareRestauranteContext());

            if (platillo != null)
            {
                IdPlatillo = platillo.IdPlatillo;
                Nombre = platillo.Nombre;
                Precio = platillo.Precio;
                ModalTitle = "Editar Platillo";

                var ingredientes = _platilloRepository.ObtenerIngredientesPlatillo(platillo.IdPlatillo);

                if (ingredientes != null && ingredientes.Count != 0)
                    CargarDesdeModelo(ingredientes);
            }
            else
                ModalTitle = "Agregar Platillo";           
        }

        public void CargarDesdeModelo(List<Producto> productosOriginales)
        {
            Productos = [.. productosOriginales.Select(p => new ProductoViewModel(p))];
        }

        public bool GuardarPlatillo(Platillo platillo)
        {
            try
            {
                if (string.IsNullOrEmpty(Nombre))
                {
                    MessageBox.Show("Los datos capturados no tienen el formato correcto.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }

                if (platillo.IdPlatillo == 0)
                {
                    if (!_platilloRepository.GuardarPlatilloConIngredientes(platillo, Productos.Select(p => p.Producto).ToList()))
                    {
                        MessageBox.Show("Ocurrió un error al agregar el platillo.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return false;
                    }

                    MessageBox.Show("Platillo guardado exitosamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {

                }
            }
            catch (Exception e)
            {
                MessageBox.Show($"Ocurrió un error al registrar los cambios en el platillo. {e.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                return false;
            }

            return true;
        }
    }

    class ProductoViewModel : ViewModelBase
    {
        public Producto Producto { get; }

        public ProductoViewModel(Producto producto)
        {
            Producto = producto;
        }

        public int IdProducto => Producto.IdProducto;

        public string Nombre => Producto.Nombre;

        public int StockActual
        {
            get => Producto.StockActual;
            set
            {
                if (Producto.StockActual != value)
                {
                    Producto.StockActual = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
