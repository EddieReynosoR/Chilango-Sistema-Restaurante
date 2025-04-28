using SistemaRestaurante.Models;
using SistemaRestaurante.Repositories;
using System.Collections.ObjectModel;

namespace SistemaRestaurante.ViewModels
{
    internal class PlatilloViewModel : ViewModelBase
    {
        /* Evitamos que se hagan cambios directos a la variable
         * original de los platillos. Asegurandonos de que se actualice esta
         * variable mediante su respectiva propiedad que ejecuta el método
         * OnPropertyChanged que asegura el buen funcionamiento de la arquitectura
         * VMMV del proyecto. */
        private ObservableCollection<Platillo> _platillos;
        public ObservableCollection<Platillo> Platillos
        {
            get => _platillos;
            set
            {
                _platillos = value;
                OnPropertyChanged(nameof(Platillos));
            }
        }

        public int CantidadPlatillos => Platillos?.Count ?? 0;

        private readonly PlatilloRepository _platilloRepository;

        public PlatilloViewModel()
        {
            _platilloRepository = new PlatilloRepository(new RestauranteDbContext());
            CargarPlatillos();
        }

        public async void CargarPlatillos()
        {
            Platillos = new ObservableCollection<Platillo>(await Task.Run(() => _platilloRepository.ObtenerPlatillos()));

            OnPropertyChanged(nameof(CantidadPlatillos));
        }
    }
}
