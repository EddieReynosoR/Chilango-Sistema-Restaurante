using SistemaRestaurante.Models;
using SistemaRestaurante.Utilities.Facade;
using SistemaRestaurante.Utilities.ReporteVentaProxy;
using System.Collections.ObjectModel;
using System.Windows;

namespace SistemaRestaurante.ViewModels
{
    class VentaViewModel : ViewModelBase
    {
        private readonly IReporteVenta _reporteVenta;
        private readonly ReporteVentaFacade _reporteVentaFacade;
        private ObservableCollection<Ventum> _ventas;

        public ObservableCollection<Ventum> Ventas
        {
            get => _ventas;
            set
            {
                _ventas = value;
                OnPropertyChanged(nameof(Ventas));
            }
        }

        private string _correo;

        public string Correo
        {
            get => _correo;
            set
            {
                _correo = value;
                OnPropertyChanged(nameof(Correo));
            }
        }

        public int CantidadVentas => Ventas?.Count ?? 0;

        public VentaViewModel()
        {
            var context = new SoftwareRestauranteContext();

            /* Nos aseguramos que utilicemos nuestro Proxy para ejecutar la clase 
             * en donde se genera el PDF */
            _reporteVentaFacade = new ReporteVentaFacade(context);
            _reporteVenta = new ReporteVentaProxy(_reporteVentaFacade);
            CargarVentas();
        }

        public async void CargarVentas()
        {
            Ventas = new ObservableCollection<Ventum>(await Task.Run(_reporteVentaFacade.ObtenerVentas));

            OnPropertyChanged(nameof(CantidadVentas));
        }

        public bool EliminarProducto(Ventum venta)
        {
            try
            {
                var result = MessageBox.Show($"¿Estás seguro de eliminar la venta #'{venta.IdVenta}'?",
                    "Confirmar eliminación", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result != MessageBoxResult.Yes)
                    return false;

                if (!_reporteVentaFacade.EliminarVenta(venta))
                {
                    MessageBox.Show("Ocurrió un error al eliminar la venta.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }

                MessageBox.Show("Venta eliminada exitosamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception e)
            {
                MessageBox.Show($"Ocurrió un error al eliminar la venta. {e.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        public bool GenerarFormatoCorteCaja(DateTime fecha)
        {
            var ventas = _reporteVentaFacade.ObtenerVentasPorFecha(fecha);

            if (ventas.Count == 0)
            {
                MessageBox.Show("No se encontraron ventas en la fecha especificada.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            var productos = _reporteVentaFacade.ObtenerProductosParaReabastecer();
            return _reporteVenta.GenerarReporteCorteCaja(ventas, productos, fecha, Correo);
        }
    }
}
