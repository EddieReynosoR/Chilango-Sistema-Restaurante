using SistemaRestaurante.Models;

namespace SistemaRestaurante.Utilities.Observer
{
    internal interface IObservadorOrden
    {
        void NotificarOrdenTardia(Orden orden, TimeSpan duracion);
        void ActualizarDuracionOrden(Orden orden, TimeSpan duracion);
    }
}
