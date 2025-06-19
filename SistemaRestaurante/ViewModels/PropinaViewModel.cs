using SistemaRestaurante.Models;
using SistemaRestaurante.Repositories;
using SistemaRestaurante.Utilities.ValidadorVenta;
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

        private string _correoElectronico;

        public string CorreoElectronico
        {
            get => _correoElectronico;
            set
            {
                if (_correoElectronico != value)
                {
                    _correoElectronico = value;
                    OnPropertyChanged(nameof(CorreoElectronico));
                }
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
            _ordenRepository = new OrdenRepository(new SoftwareRestauranteContext());
            PorcentajePropina = 0.1m;
        }

        public bool GenerarVenta(decimal propina)
        {
            try
            {
                var validador1 = new VentaNoVaciaValidator();
                var validador2 = new StockSuficienteValidator(_ordenRepository);

                validador1.SetNext(validador2);
                validador1.Validate(Platillos);

                if (!_ordenRepository.GenerarVenta(Platillos, IdOrden, propina))
                {
                    MessageBox.Show($"Ocurrió un error al generar la venta.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al generar la venta: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
    }
}
