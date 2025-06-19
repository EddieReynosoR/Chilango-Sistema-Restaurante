using SistemaRestaurante.Models;
using SistemaRestaurante.Repositories;
using System.Windows.Threading;

namespace SistemaRestaurante.Utilities.Observer
{
    internal class TemporizadorOrdenes
    {
        private readonly List<IObservadorOrden> _observadores = [];
        private readonly List<Orden> _ordenes = [];
        private readonly TimeSpan _limite = TimeSpan.FromSeconds(30);
        private readonly DispatcherTimer _timer;
        private readonly OrdenRepository _ordenRepository;

        public TemporizadorOrdenes()
        {
            _ordenRepository = new OrdenRepository(new SoftwareRestauranteContext());

            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _timer.Tick += EvaluarOrdenes;
            _timer.Start();
        }

        public void RegistrarObservador(IObservadorOrden observador)
        {
            if (!_observadores.Contains(observador))
                _observadores.Add(observador);
        }

        public void AgregarOrden(Orden orden)
        {
            if (!_ordenes.Contains(orden))
                _ordenes.Add(orden);
        }

        public void RemoverOrden(int idOrden)
        {
            var orden = _ordenes.FirstOrDefault(o => o.IdOrden == idOrden);
            if (orden != null)
                _ordenes.Remove(orden);
        }

        private void EvaluarOrdenes(object? sender, EventArgs e)
        {
            foreach (var orden in _ordenes.ToList())
            {
                var duracion = DateTime.Now - orden.Fecha;

                foreach (var obs in _observadores)
                    obs.ActualizarDuracionOrden(orden, duracion);

                if (!orden.EsTardio && duracion > _limite)
                {
                    orden.EsTardio = true;
                    orden.SegundosTardio = (int)duracion.TotalSeconds;

                    if (_ordenRepository.GuardarCambiosOrden(orden))
                    {
                        foreach (var obs in _observadores)
                            obs.NotificarOrdenTardia(orden, duracion);
                    }
                }
            }
        }
    }
}
