using SistemaRestaurante.Models;
using SistemaRestaurante.Repositories;
using System.Collections.ObjectModel;
using System.Windows;

namespace SistemaRestaurante.ViewModels
{
    internal class OrdenViewModel : ViewModelBase
    {
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

        public int IdOrden { get; set; }
        public string TituloOrden { get; set; }


        private readonly OrdenRepository _ordenRepository;

        public OrdenViewModel(Orden orden)
        {
            _ordenRepository = new OrdenRepository(new RestauranteDbContext());
            IdOrden = orden.IdOrden;
            TituloOrden = $"Orden ID #{orden.IdOrden}";
        }

        public void CargarPlatillosGuardados()
        {
            try
            {
                var platillos = _ordenRepository.ObtenerPlatillosGuardados(IdOrden);

                if (platillos != null)
                    Platillos = [.. platillos];
            }
            catch (Exception e)
            {
                MessageBox.Show($"Ocurrió un error al consultar los platillos guardados. {e.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public bool GuardarPlatillosOrden()
        {
            try
            {
                if (!_ordenRepository.GuardarPlatillosOrden(Platillos, IdOrden))
                {
                    MessageBox.Show("Ocurrió un error al guardar la orden.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }

                MessageBox.Show("Orden guardada correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception e)
            {
                MessageBox.Show($"Ocurrió un error al guardar la orden. {e.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        public bool EliminarPlatillo(PlatilloSeleccionado platilloSeleccionado)
        {
            try
            {
                var platillo = Platillos.FirstOrDefault(p => p.Platillo.IdPlatillo == platilloSeleccionado.Platillo.IdPlatillo);

                if (platillo == null)
                {
                    MessageBox.Show($"Ocurrió un error al eliminar el platillo.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
                
                Platillos.Remove(platillo);              

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ocurrió un error al eliminar el platillo: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public bool CancelarOrden()
        {
            try
            {
                if (!_ordenRepository.CancelarOrden(IdOrden))
                {
                    MessageBox.Show($"Ocurrió un error al cancelar el platillo.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
                
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ocurrió un error al eliminar el platillo: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
    }
}
