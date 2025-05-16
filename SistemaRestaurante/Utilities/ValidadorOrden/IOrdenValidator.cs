using SistemaRestaurante.ViewModels;
using System.Collections.ObjectModel;

namespace SistemaRestaurante.Utilities.ValidadorOrden
{
    internal interface IOrdenValidator
    {
        IOrdenValidator SetNext(IOrdenValidator next);
        void Validate(ObservableCollection<PlatilloSeleccionado> platillos);
    }
}
