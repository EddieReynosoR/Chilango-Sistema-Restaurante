using SistemaRestaurante.Models;
using SistemaRestaurante.Repositories;
using System.Collections.ObjectModel;
using System.Windows;

namespace SistemaRestaurante.Utilities.Facade
{
    internal class RestauranteFacade
    {
        private readonly MesaRepository _mesaRepository;
        private readonly OrdenRepository _ordenRepository;

        public RestauranteFacade(RestauranteDbContext context)
        {
            _mesaRepository = new MesaRepository(context);
            _ordenRepository = new OrdenRepository(context);
        }

        public ObservableCollection<Mesa> ObtenerMesas()
        {
            return [.. _mesaRepository.ObtenerMesas()];
        }

        public Orden? ExisteOrden(int idMesa)
        {
            return _ordenRepository.ExisteOrdenEnMesa(idMesa);
        }

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

        public bool EliminarMesa(Mesa mesa)
        {
            try
            {
                if (_ordenRepository.ExisteOrdenEnMesa(mesa.IdMesa) != null)
                {
                    MessageBox.Show("Hay una orden activa en la mesa. Eliminala para eliminar la mesa.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }

                mesa.Estatus = false;

                return _mesaRepository.GuardarCambios(mesa);
            }
            catch (Exception)
            {
                return false;
            }
        }

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
