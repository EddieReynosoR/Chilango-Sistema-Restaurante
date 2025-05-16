using SistemaRestaurante.Utilities.ValidadorOrden;
using SistemaRestaurante.ViewModels;
using System.Collections.ObjectModel;

internal abstract class OrdenValidatorBase : IOrdenValidator
{
    private IOrdenValidator _next;

    public IOrdenValidator SetNext(IOrdenValidator next)
    {
        _next = next;
        return next;
    }

    public virtual void Validate(ObservableCollection<PlatilloSeleccionado> platillos)
    {
        _next?.Validate(platillos);
    }
}
