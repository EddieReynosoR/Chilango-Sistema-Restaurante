using SistemaRestaurante.Models;
using SistemaRestaurante.Repositories;
using System.Collections.ObjectModel;
using System.Windows;
using static SistemaRestaurante.Utilities.GlobalUtilities;

namespace SistemaRestaurante.ViewModels
{
    class AgregarPlatilloOrdenViewModel : ViewModelBase
    {
        private readonly PlatilloRepository _platilloRepository;

        private AreaPlatillo _areaSeleccionada;
        private ObservableCollection<PlatilloSeleccionado> _platillos;
        private ObservableCollection<PlatilloSeleccionado> _listaPlatillosSeleccionados;

        public ObservableCollection<AreaPlatillo> Areas { get; } =
        [
            AreaPlatillo.Cocina,
            AreaPlatillo.Bebidas
        ];

        public AreaPlatillo AreaSeleccionada
        {
            get => _areaSeleccionada;
            set
            {
                if (_areaSeleccionada != value)
                {
                    _areaSeleccionada = value;
                    OnPropertyChanged();
                    CargarPlatillos(_areaSeleccionada);
                }
            }
        }

        public ObservableCollection<PlatilloSeleccionado> Platillos
        {
            get => _platillos;
            set
            {
                _platillos = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<PlatilloSeleccionado> ListaPlatillosSeleccionados
        {
            get => _listaPlatillosSeleccionados;
            set
            {
                _listaPlatillosSeleccionados = value;
                OnPropertyChanged(nameof(ListaPlatillosSeleccionados));
            }
        }

        public List<PlatilloSeleccionado>? PlatillosSeleccionados => Platillos?.Where(p => p.IsSelected).ToList();

        public AgregarPlatilloOrdenViewModel()
        {
            _platilloRepository = new PlatilloRepository(new SoftwareRestauranteContext());
            AreaSeleccionada = AreaPlatillo.Cocina;
        }

        public void CargarPlatillos(AreaPlatillo area) 
        { 
            Platillos = [.. _platilloRepository.ObtenerPlatillos(area).Select(p => new PlatilloSeleccionado(p))]; 
        }

        public bool GuardarPlatillo()
        {
            try
            {
                var platillos = PlatillosSeleccionados;

                if (platillos?.Any() != true)
                {
                    MessageBox.Show("No se seleccionaron platillos.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }

                ListaPlatillosSeleccionados = [.. platillos];

                MessageBox.Show("Se agregaron los platillos correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception e)
            {
                MessageBox.Show($"Ocurrió un error al asignar los platillos a la orden. {e.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                return false;
            }

            return true;
        }
    }   

    internal class PlatilloSeleccionado : ViewModelBase
    {
        public Platillo Platillo { get; }

        private bool _isSelected;
        private int _cantidad = 1;

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

        public int Cantidad
        {
            get => _cantidad;
            set
            {
                if (_cantidad != value)
                {
                    _cantidad = value;
                    OnPropertyChanged(nameof(Cantidad));
                }
            }
        }

        public PlatilloSeleccionado(Platillo platillo)
        {
            Platillo = platillo;
        }
    }
}
