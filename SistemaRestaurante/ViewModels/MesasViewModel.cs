using SistemaRestaurante.Models;
using SistemaRestaurante.Utilities.Facade;
using SistemaRestaurante.Utilities.Observer;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Data;

namespace SistemaRestaurante.ViewModels
{
    internal class MesasViewModel : ViewModelBase, IObservadorOrden
    {
        private Dictionary<int, TimeSpan> _duracionOrdenes = [];

        private ObservableCollection<MesaWrapper> _mesas;
        public ObservableCollection<MesaWrapper> Mesas
        {
            get => _mesas;
            set
            {
                _mesas = value;
                OnPropertyChanged(nameof(Mesas));
            }
        }

        public int CantidadMesas
        {
            get => Mesas?.Count ?? 0;
            set => OnPropertyChanged(nameof(CantidadMesas));
        }

        private readonly RestauranteFacade _restauranteFacade;

        public MesasViewModel(TemporizadorOrdenes temporizador)
        {
            _restauranteFacade = new RestauranteFacade(new SoftwareRestauranteContext());
            CargarMesas();

            temporizador.RegistrarObservador(this);
        }

        public async void CargarMesas()
        {
            var mesas = await Task.Run(_restauranteFacade.ObtenerMesas);
            Mesas = [.. mesas.Select(m => new MesaWrapper(m))];
            CantidadMesas = Mesas.Count;
        }

        public Orden? ExisteOrden(int idMesa) => _restauranteFacade.ExisteOrden(idMesa);

        public Orden? ActivarMesa(MesaWrapper mesa)
        {
            Orden? orden;

            try
            {
                orden = _restauranteFacade.AbrirOrdenParaMesa(mesa.Mesa);

                if (orden == null)
                {
                    MessageBox.Show("Ocurrió un error al abrir la orden.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return null;
                }

                mesa.OrdenActiva = orden;
                App.TemporizadorOrdenes?.AgregarOrden(orden);
                MessageBox.Show("Orden creada correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception e)
            {
                MessageBox.Show($"Ocurrió un error al abrir la orden. {e.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }

            return orden;
        }

        public bool EliminarMesa(Mesa mesa)
        { 
            try
            {
                var result = MessageBox.Show($"¿Estás seguro de eliminar la mesa #'{mesa.Numero}'?",
                    "Confirmar eliminación", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result != MessageBoxResult.Yes)
                    return false;

                if (!_restauranteFacade.EliminarMesa(mesa))
                {
                    MessageBox.Show("Ocurrió un error al eliminar la mesa.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
                
                MessageBox.Show("Mesa eliminada exitosamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception e)
            {
                MessageBox.Show($"Ocurrió un error al eliminar la mesa. {e.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        public bool AgregarMesa(int numeroMesa)
        {
            try
            {
                if (!_restauranteFacade.AgregarMesa(numeroMesa))
                {
                    MessageBox.Show("Ocurrió un error al agregar la mesa.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                    return false;
                }

                MessageBox.Show("Mesa agregada exitosamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception e)
            {
                MessageBox.Show($"Ocurrió un error al agregar la mesa. {e.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                return false;
            }

            return true;
        }

        public void NotificarOrdenTardia(Orden orden, TimeSpan duracion)
        {
            MessageBox.Show($"Orden #{orden.IdOrden} superó los {duracion.TotalSeconds:F1} segundos.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public void ActualizarDuracionOrden(Orden orden, TimeSpan duracion)
        {
            _duracionOrdenes[orden.IdOrden] = duracion;

            Application.Current.Dispatcher.Invoke(() =>
            {
                var wrapper = Mesas.FirstOrDefault(m => m.OrdenActiva?.IdOrden == orden.IdOrden);
                if (wrapper != null)
                {
                    wrapper.TiempoOrden = $"{(int)duracion.TotalMinutes:D2}:{duracion.Seconds:D2}";
                }

                CollectionViewSource.GetDefaultView(Mesas)?.Refresh();
            });
        }
    }

    public class MesaWrapper
    {
        public Mesa Mesa { get; set; }

        public string? TiempoOrden { get; set; }

        public bool Ocupada => Mesa.Ocupada;
        public int IdMesa => Mesa.IdMesa;
        public int Numero => Mesa.Numero;

        public Orden? OrdenActiva { get; set; }

        public MesaWrapper(Mesa mesa)
        {
            Mesa = mesa;
        }
    }
}
