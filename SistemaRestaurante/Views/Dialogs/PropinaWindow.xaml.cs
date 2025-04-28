using SistemaRestaurante.ViewModels;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace SistemaRestaurante.Views.Dialogs
{
    /// <summary>
    /// Interaction logic for PropinaWindow.xaml
    /// </summary>
    public partial class PropinaWindow : Window
    {
        private readonly PropinaViewModel _viewModel;

        internal ObservableCollection<PlatilloSeleccionado> Platillos { get; set; }

        internal int IdOrden { get; set; }

        public PropinaWindow()
        {
            InitializeComponent();
            _viewModel = new PropinaViewModel();
            DataContext = _viewModel;
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton radioButton && decimal.TryParse(radioButton.Tag?.ToString(), out decimal porcentaje))
                _viewModel.PorcentajePropina = porcentaje;
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.Platillos = Platillos;
            _viewModel.IdOrden = IdOrden;

            if (!_viewModel.GenerarVenta(_viewModel.PorcentajePropina))
                return;

            MessageBox.Show("Venta generada correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

            DialogResult = true;
            Close();
        }
    }
}
