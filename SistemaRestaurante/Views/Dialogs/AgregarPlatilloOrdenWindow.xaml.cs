using SistemaRestaurante.Models;
using SistemaRestaurante.ViewModels;
using System.Collections.ObjectModel;
using System.Windows;

namespace SistemaRestaurante.Views.Dialogs
{
    /// <summary>
    /// Interaction logic for AgregarPlatilloOrden.xaml
    /// </summary>
    public partial class AgregarPlatilloOrdenWindow : Window
    {
        private readonly AgregarPlatilloOrdenViewModel _viewModel;

        internal ObservableCollection<PlatilloSeleccionado> PlatillosSeleccionadosResultado { get; private set; }

        public AgregarPlatilloOrdenWindow()
        {
            InitializeComponent();

            _viewModel = new AgregarPlatilloOrdenViewModel();
            DataContext = _viewModel;

            _viewModel.CargarPlatillos(_viewModel.AreaSeleccionada);
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.GuardarPlatillo();

            PlatillosSeleccionadosResultado = _viewModel.ListaPlatillosSeleccionados;
            DialogResult = true;
            Close();
        }
    }
}
