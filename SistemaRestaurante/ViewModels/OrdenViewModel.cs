using SistemaRestaurante.Models;
using SistemaRestaurante.Repositories;
using SistemaRestaurante.Utilities.ValidadorOrden;
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
            _ordenRepository = new OrdenRepository(new SoftwareRestauranteContext());
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
                var validador1 = new NoVacioValidator();
                var validador2 = new CantidadValidaValidator();
                var validador3 = new SinDuplicadosValidator();

                validador1
                    .SetNext(validador2)
                    .SetNext(validador3);

                validador1.Validate(Platillos);

                if (!_ordenRepository.GuardarPlatillosOrden(Platillos, IdOrden))
                {
                    MessageBox.Show("Ocurrió un error al guardar la orden.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }

                MessageBox.Show("Orden guardada correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show($"Error de validación: {e.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
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
                var viewModel = App.MesasViewModel;

                if (!_ordenRepository.CancelarOrden(IdOrden))
                {
                    MessageBox.Show($"Ocurrió un error al cancelar la orden", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }

                App.TemporizadorOrdenes.RemoverOrden(IdOrden);

                var mesa = viewModel.Mesas.FirstOrDefault(m => m.OrdenActiva != null && m.OrdenActiva.IdOrden == IdOrden);

                if (mesa == null)
                {
                    MessageBox.Show($"Ocurrió un error al cancelar la orden.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }

                mesa.OrdenActiva = null;
                mesa.TiempoOrden = null;
                mesa.Mesa.Ocupada = false;

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
