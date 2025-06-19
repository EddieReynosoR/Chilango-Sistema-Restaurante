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
            _ventasRepository = new VentasRepository(new SoftwareRestauranteContext());

            var usuario = Session.Instance;
            UsuarioNombre = UsuarioNombre = $"Hello, {usuario?.Nombre ?? "Desconocido"}";
            UsuarioRol = usuario?.Rol ?? "Sin rol";
        }
    }
}
