using SistemaRestaurante.Models;
using SistemaRestaurante.Repositories;
using System.Windows;

namespace SistemaRestaurante.ViewModels
{
    internal class AgregarEditarProductoViewModel : ViewModelBase
    {
        private int _idProducto;
        private string _nombre;
        private int _minimo;
        private int _stock;
        private string _modalTitle;

        private readonly ProductoRepository _productoRepository;

        public int IdProducto
        {
            get => _idProducto;
            set
            {
                _idProducto = value;
                OnPropertyChanged(nameof(IdProducto));
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

        public int Minimo
        {
            get => _minimo;
            set 
            {
                _minimo = value;
                OnPropertyChanged(nameof(Minimo));
            }
        }

        public int Stock
        {
            get => _stock;
            set
            {
                _stock = value;
                OnPropertyChanged(nameof(Stock));
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

        public AgregarEditarProductoViewModel(Producto? producto = null)
        {
            if (producto != null)
            {
                IdProducto = producto.IdProducto;
                Nombre = producto.Nombre;
                Stock = producto.StockActual;
                Minimo = producto.Minimo;
                ModalTitle = "Editar Producto";
            }
            else
                ModalTitle = "Agregar Producto";

            _productoRepository = new ProductoRepository(new RestauranteDbContext());
        }

        public bool GuardarProducto()
        {
            try
            {
                if (Stock == 0 || Minimo == 0 || string.IsNullOrEmpty(Nombre))
                {
                    MessageBox.Show("Los datos capturados no tienen el formato correcto.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                    return false;
                }

                var producto = new Producto
                {
                    IdProducto = IdProducto,
                    Nombre = Nombre,
                    StockActual = Stock,
                    Minimo = Minimo
                };

                if (producto.IdProducto == 0)
                {
                    if (!_productoRepository.AgregarProducto(producto))
                    {
                        MessageBox.Show("Ocurrió un error al agregar el producto.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                        return false;
                    }

                    MessageBox.Show("Producto guardado exitosamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    if (!_productoRepository.EditarProducto(producto))
                    {
                        MessageBox.Show("Ocurrió un error al editar el producto.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                        return false;
                    }

                    MessageBox.Show("Producto editado exitosamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show($"Ocurrió un error al registrar los cambios en el producto. {e.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                return false;
            }           

            return true;
        }
    }
}
