using SistemaRestaurante.Models;
using System.Windows;

namespace SistemaRestaurante.Utilities.ReporteVentaProxy
{
    internal class ReporteVentaProxy : IReporteVenta
    {
        private readonly IReporteVenta _reporte;
        private bool _hayCambios = true;
        private DateTime _fechaGenerada = DateTime.MinValue;

        public ReporteVentaProxy(IReporteVenta reporteReal)
        {
            _reporte = reporteReal;
        }

        public bool GenerarReporteCorteCaja(List<Ventum> ventas, List<(Producto Producto, int CantidadFaltante)> productos, DateTime fecha)
        {
            // Utilizando este proxy, podemos asegurar de que no generemos el PDF de forma innecesaria, al menos de que se indique otra fecha.
            // O se salga y entre a de nuevo al mismo apartado.
            if (!_hayCambios && fecha.Date == _fechaGenerada.Date)
            {
                MessageBox.Show("El reporte ya está generado. No es necesario generarlo de nuevo.", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
                return false;
            }

            // Con el Proxy aseguramos que solo un administrador pueda generar el corte de caja.
            if (!Session.EsAdmin())
            {
                MessageBox.Show("No tienes permisos para generar reportes.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            bool resultado = _reporte.GenerarReporteCorteCaja(ventas, productos, fecha);

            if (resultado)
            {
                _hayCambios = false;
                _fechaGenerada = fecha;
            }                

            return resultado;
        }
    }
}
