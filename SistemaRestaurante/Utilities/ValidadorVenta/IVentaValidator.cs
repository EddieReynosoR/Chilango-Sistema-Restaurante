using SistemaRestaurante.ViewModels;
using System.Collections.ObjectModel;

namespace SistemaRestaurante.Utilities.ValidadorVenta
{
    internal interface IVentaValidator
    {
        IVentaValidator SetNext(IVentaValidator next);
        void Validate(ObservableCollection<PlatilloSeleccionado> platillos);
    }
}
