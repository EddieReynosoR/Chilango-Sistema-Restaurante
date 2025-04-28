using SistemaRestaurante.Models;
using SistemaRestaurante.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace SistemaRestaurante.Views.Dialogs
{
    /// <summary>
    /// Interaction logic for AgregarOrdenMesa.xaml
    /// </summary>
    public partial class OrdenMesaWindow : Window
    {
        private readonly OrdenViewModel _viewModel;

        public OrdenMesaWindow(Orden orden)
        {
            InitializeComponent();

            _viewModel = new OrdenViewModel(orden);
            DataContext = _viewModel;

            _viewModel.CargarPlatillosGuardados();
        }

        private void btnAgregarOrden_Click(object sender, RoutedEventArgs e)
        {
            var modal = new AgregarPlatilloOrdenWindow();

            bool? resultado = modal.ShowDialog();

            if (resultado == true)
            {
                foreach (var platilloSeleccionado in modal.PlatillosSeleccionadosResultado)
                {
                    var platilloExistente = _viewModel.Platillos
                        .FirstOrDefault(p => p.Platillo.IdPlatillo == platilloSeleccionado.Platillo.IdPlatillo);

                    if (platilloExistente != null)
                        platilloExistente.Cantidad += platilloSeleccionado.Cantidad;
                    else                   
                        _viewModel.Platillos.Add(platilloSeleccionado);
                }
            }
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.GuardarPlatillosOrden();

            DialogResult = true;
            Close();
        }

        private void btnVenta_Click(object sender, RoutedEventArgs e)
        {
            var modal = new PropinaWindow();
            modal.Platillos = _viewModel.Platillos;
            modal.IdOrden = _viewModel.IdOrden;

            bool? result = modal.ShowDialog();

            if (result == true)
                Close();
        }

        private void btnCancelarOrden_Click(object sender, RoutedEventArgs e)
        {
            var resultado = MessageBox.Show(
                "¿Estás seguro de que quieres cancelar esta orden? Esta acción no se puede deshacer.",
                "Confirmar cancelación",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (resultado != MessageBoxResult.Yes)
                return;

            if (!_viewModel.CancelarOrden())
                return;

            MessageBox.Show("Orden cancelada correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            Close();
        }

        private void btnAumentar_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            if (button?.Tag is PlatilloSeleccionado platillo)
                platillo.Cantidad++;
        }

        private void btnDisminuir_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            if (button?.Tag is PlatilloSeleccionado platillo && platillo.Cantidad > 1)
                platillo.Cantidad--;
        }

        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            if (button?.Tag is PlatilloSeleccionado platillo)
            {
                if (!_viewModel.EliminarPlatillo(platillo))
                {
                    MessageBox.Show("No se pudo eliminar el platillo.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                MessageBox.Show("Platillo eliminado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
