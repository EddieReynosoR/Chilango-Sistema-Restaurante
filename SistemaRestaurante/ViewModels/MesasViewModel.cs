using SistemaRestaurante.Models;
using SistemaRestaurante.Utilities.Facade;
using System.Collections.ObjectModel;
using System.Windows;

namespace SistemaRestaurante.ViewModels
{
    internal class MesasViewModel : ViewModelBase
    {
        private ObservableCollection<Mesa> _mesas;

        public ObservableCollection<Mesa> Mesas
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

        public MesasViewModel()
        {
            _restauranteFacade = new RestauranteFacade(new RestauranteDbContext());
            CargarMesas();
        }

        public async void CargarMesas()
        {
            Mesas = [.. await Task.Run(_restauranteFacade.ObtenerMesas)];
            CantidadMesas = Mesas.Count;
        }

        public Orden? ExisteOrden(int idMesa) => _restauranteFacade.ExisteOrden(idMesa);

        public Orden? ActivarMesa(Mesa mesa)
        {
            Orden? orden;

            try
            {
                orden = _restauranteFacade.AbrirOrdenParaMesa(mesa);

                if (orden == null)
                {
                    MessageBox.Show("Ocurrió un error al abrir la orden.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return null;
                }

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
    }
}
