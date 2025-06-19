using SistemaRestaurante.Models;

namespace SistemaRestaurante.Utilities.ValidarProducto
{
    internal class ValidadorNombre : IValidadorProducto
    {
        public IValidadorProducto Next { get; set; }

        public bool Validar(Producto producto, out string mensajeError)
        {
            if (string.IsNullOrWhiteSpace(producto.Nombre))
            {
                mensajeError = "El nombre del producto no puede estar vacío.";
                return false;
            }

            return Next?.Validar(producto, out mensajeError) ?? (mensajeError = null) == null;
        }
    }

    internal class ValidadorStock : IValidadorProducto
    {
        public IValidadorProducto Next { get; set; }

        public bool Validar(Producto producto, out string mensajeError)
        {
            if (producto.StockActual <= 0)
            {
                mensajeError = "El stock debe ser mayor que cero.";
                return false;
            }

            return Next?.Validar(producto, out mensajeError) ?? (mensajeError = null) == null;
        }
    }

    internal class ValidadorMinimo : IValidadorProducto
    {
        public IValidadorProducto Next { get; set; }

        public bool Validar(Producto producto, out string mensajeError)
        {
            if (producto.Minimo <= 0)
            {
                mensajeError = "El mínimo debe ser mayor que cero.";
                return false;
            }

            return Next?.Validar(producto, out mensajeError) ?? (mensajeError = null) == null;
        }
    }
}
