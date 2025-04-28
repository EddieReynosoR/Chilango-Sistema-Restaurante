using SistemaRestaurante.Models;
using SistemaRestaurante.Repositories;
using System.Collections.ObjectModel;
using System.Windows;

namespace SistemaRestaurante.Utilities
{
    internal class RestauranteFacade
    {
        // Uso de par de repositorios para realizar operaciones en base de datos.
        // (Potencialmente pueden agregarse más métodos.
        private readonly MesaRepository _mesaRepository;
        private readonly OrdenRepository _ordenRepository;

        public RestauranteFacade(RestauranteDbContext context)
        {
            _mesaRepository = new MesaRepository(context);
            _ordenRepository = new OrdenRepository(context);
        }

        // Método utilizado para consultar las mesas en base de datos.
        public ObservableCollection<Mesa> ObtenerMesas()
        {
            return [.. _mesaRepository.ObtenerMesas()];
        }

        // Método utilizado consultar si una mesa ya tiene alguna orden activa asignada.
        public Orden? ExisteOrden(int idMesa)
        {
            return _ordenRepository.ExisteOrdenEnMesa(idMesa);
        }

        // Método utilizado para asignar una nueva orden a una mesa seleccionada.
        public Orden? AbrirOrdenParaMesa(Mesa mesa)
        {
            try
            {
                mesa.Ocupada = true;
                if (!_mesaRepository.GuardarCambios(mesa))
                    return null;

                var ordenNueva = new Orden
                {
                    MesaId = mesa.IdMesa,
                    Fecha = DateTime.Now,
                    IdUsuario = Session.Instance.IdUsuario,
                    Estatus = true
                };

                if (!_ordenRepository.AgregarOrden(ordenNueva))
                    return null;

                return ordenNueva;
            }
            catch (Exception)
            {
                return null;
            }
        }

        // Método utilizado para eliminar una mesa.
        public bool EliminarMesa(Mesa mesa)
        {
            try
            {
                if (_ordenRepository.ExisteOrdenEnMesa(mesa.IdMesa) != null)
                {
                    MessageBox.Show("Hay una orden activa en la mesa. Eliminala para eliminar la mesa.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }

                return _mesaRepository.EliminarMesa(mesa);
            }
            catch (Exception)
            {
                return false;
            }
        }

        // Método para agregar una mesa nueva.
        public bool AgregarMesa(int numeroMesa)
        {           
            try
            {
                return _mesaRepository.AgregarMesa(numeroMesa);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
