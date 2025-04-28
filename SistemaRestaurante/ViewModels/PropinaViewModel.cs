using SistemaRestaurante.Models;
using SistemaRestaurante.Repositories;
using System.Collections.ObjectModel;
using System.Windows;

namespace SistemaRestaurante.ViewModels
{
    class PropinaViewModel : ViewModelBase
    {
        private readonly OrdenRepository _ordenRepository;

        private ObservableCollection<PlatilloSeleccionado> _platillos = [];
        public ObservableCollection<PlatilloSeleccionado> Platillos
        {
            get => _platillos;
            set
            {
                _platillos = value;
                OnPropertyChanged(nameof(Platillos));
            }
        }

        private decimal _porcentajePropina;

        public decimal PorcentajePropina
        {
            get => _porcentajePropina;
            set
            {
                if (_porcentajePropina != value)
                {
                    _porcentajePropina = value;
                    OnPropertyChanged(nameof(PorcentajePropina));
                }
            }
        }

        public int IdOrden { get; set; }

        public PropinaViewModel()
        {
            _ordenRepository = new OrdenRepository(new RestauranteDbContext());
            PorcentajePropina = 0.1m;
        }

        public bool GenerarVenta(decimal propina)
        {
            try
            {
                if (Platillos == null || !Platillos.Any())
                {
                    MessageBox.Show($"No se detectaron platillos para realizar la venta.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }

                if (!_ordenRepository.ValidarCantidadProductos(Platillos))
                {
                    MessageBox.Show($"No hay productos suficientes para la orden.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }

                if (!_ordenRepository.GenerarVenta(Platillos, IdOrden, propina))
                {
                    MessageBox.Show($"Ocurrió un error al generar la venta.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ocurrió un error al generar la venta: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
    }
}
