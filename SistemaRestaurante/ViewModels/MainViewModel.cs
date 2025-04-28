using LiveCharts;
using LiveCharts.Wpf;
using SistemaRestaurante.Models;
using SistemaRestaurante.Repositories;
using SistemaRestaurante.Utilities;

namespace SistemaRestaurante.ViewModels
{
    internal class MainViewModel : ViewModelBase
    {
        private readonly VentasRepository _ventasRepository;

        private string _usuarioNombre;
        private string _usuarioRol;

        private List<Ventum> _ventas;
        private SeriesCollection _ventasSeries;

        public SeriesCollection VentasSeries
        {
            get { return _ventasSeries; }
            set { SetProperty(ref _ventasSeries, value, nameof(VentasSeries)); }
        }

        public string UsuarioNombre
        {
            get => _usuarioNombre;
            set
            {
                _usuarioNombre = value;
                OnPropertyChanged(nameof(UsuarioNombre));
            }
        }

        public string UsuarioRol
        {
            get => _usuarioRol;
            set
            {
                _usuarioRol = value;
                OnPropertyChanged(nameof(UsuarioRol));
            }
        }

        public MainViewModel()
        {
            _ventasRepository = new VentasRepository(new RestauranteDbContext());

            var usuario = Session.Instance;
            UsuarioNombre = UsuarioNombre = $"Hello, {usuario?.Nombre ?? "Desconocido"}";
            UsuarioRol = usuario?.Rol ?? "Sin rol";

            //CargarDatosVentas();
        }

        //private void CargarDatosVentas()
        //{
        //    _ventas = _ventasRepository.ObtenerVentas();

        //    var ventasPorDia = _ventas
        //        .GroupBy(v => v.FechaVenta.Date)
        //        .Select(g => new { Fecha = g.Key, TotalVenta = g.Sum(v => v.Total) })
        //        .ToList();

        //    var fechas = ventasPorDia.Select(v => v.Fecha.ToOADate()).ToArray();
        //    var totalesVentas = ventasPorDia.Select(v => (double)v.TotalVenta).ToArray();

        //    VentasSeries = new SeriesCollection
        //    {
        //        new LineSeries
        //        {
        //            Title = "Ventas por Día",
        //            Values = new ChartValues<double>(totalesVentas),
        //            Fill = System.Windows.Media.Brushes.Transparent,
        //            StrokeThickness = 3,
        //            PointGeometrySize = 0,
        //            Stroke = new System.Windows.Media.LinearGradientBrush(
        //                new System.Windows.Media.GradientStopCollection
        //                {
        //                    new System.Windows.Media.GradientStop(System.Windows.Media.Color.FromArgb(255, 255, 255, 255), 0.06),
        //                    new System.Windows.Media.GradientStop(System.Windows.Media.Color.FromArgb(255, 40, 137, 252), 0.5),
        //                    new System.Windows.Media.GradientStop(System.Windows.Media.Color.FromArgb(255, 255, 255, 255), 0.93)
        //                })
        //        }
        //    };
        //}
    }
}
