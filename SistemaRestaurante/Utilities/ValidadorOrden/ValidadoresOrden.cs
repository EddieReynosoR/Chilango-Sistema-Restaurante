using SistemaRestaurante.ViewModels;
using System.Collections.ObjectModel;

namespace SistemaRestaurante.Utilities.ValidadorOrden
{
    internal class NoVacioValidator : OrdenValidatorBase
    {
        public override void Validate(ObservableCollection<PlatilloSeleccionado> platillos)
        {
            if (platillos == null || !platillos.Any())
                throw new Exception("La orden no contiene platillos.");

            base.Validate(platillos);
        }
    }

    internal class CantidadValidaValidator : OrdenValidatorBase
    {
        public override void Validate(ObservableCollection<PlatilloSeleccionado> platillos)
        {
            if (platillos.Any(p => p.Cantidad <= 0))
                throw new Exception("Uno o más platillos tienen cantidad inválida.");

            base.Validate(platillos);
        }
    }

    internal class SinDuplicadosValidator : OrdenValidatorBase
    {
        public override void Validate(ObservableCollection<PlatilloSeleccionado> platillos)
        {
            var duplicados = platillos
                .GroupBy(p => p.Platillo.IdPlatillo)
                .Where(g => g.Count() > 1)
                .Any();

            if (duplicados)
                throw new Exception("La orden contiene platillos duplicados.");

            base.Validate(platillos);
        }
    }
}
