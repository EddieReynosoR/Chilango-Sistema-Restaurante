using SistemaRestaurante.ViewModels;
using System.Collections.ObjectModel;

namespace SistemaRestaurante.Utilities.ValidadorVenta
{
    internal abstract class VentaValidatorBase : IVentaValidator
    {
        private IVentaValidator _next;

        public IVentaValidator SetNext(IVentaValidator next)
        {
            _next = next;
            return next;
        }

        public virtual void Validate(ObservableCollection<PlatilloSeleccionado> platillos)
        {
            _next?.Validate(platillos);
        }
    }
}
