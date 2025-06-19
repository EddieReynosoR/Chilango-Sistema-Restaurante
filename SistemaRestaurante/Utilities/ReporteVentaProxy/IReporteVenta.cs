using SistemaRestaurante.Models;

namespace SistemaRestaurante.Utilities.ReporteVentaProxy
{
    // Interfaz utilizada para indicar los métodos a utilizar en nuestro Proxy
    internal interface IReporteVenta
    {
        bool GenerarReporteCorteCaja(List<Ventum> ventas, List<(Producto Producto, int CantidadFaltante)> productos, DateTime fecha, string? correo = null);
    }
}
