using SistemaRestaurante.Models;

namespace SistemaRestaurante.Utilities.ValidarProducto
{
    public interface IValidadorProducto
    {
        IValidadorProducto Next { get; set; }
        bool Validar(Producto producto, out string mensajeError);
    }
}
