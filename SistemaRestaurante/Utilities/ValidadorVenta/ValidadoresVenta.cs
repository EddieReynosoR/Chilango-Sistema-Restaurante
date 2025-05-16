using SistemaRestaurante.Repositories;
using SistemaRestaurante.ViewModels;
using System.Collections.ObjectModel;

namespace SistemaRestaurante.Utilities.ValidadorVenta
{
    internal class VentaNoVaciaValidator : VentaValidatorBase
    {
        public override void Validate(ObservableCollection<PlatilloSeleccionado> platillos)
        {
            if (platillos == null || !platillos.Any())
                throw new Exception("No se detectaron platillos para realizar la venta.");

            base.Validate(platillos);
        }
    }

    internal class StockSuficienteValidator : VentaValidatorBase
    {
        private readonly OrdenRepository _ordenRepository;

        public StockSuficienteValidator(OrdenRepository ordenRepository)
        {
            _ordenRepository = ordenRepository;
        }

        public override void Validate(ObservableCollection<PlatilloSeleccionado> platillos)
        {
            if (!_ordenRepository.ValidarCantidadProductos(platillos))
                throw new Exception("No hay productos suficientes para la orden.");

            base.Validate(platillos);
        }
    }
}
