using SistemaRestaurante.ViewModels;
using SistemaRestaurante.Views.Dialogs;
using System.Windows;
using System.Windows.Controls;

namespace SistemaRestaurante.Views.Pages
{
    /// <summary>
    /// Interaction logic for MesasPage.xaml
    /// </summary>
    public partial class MesasPage : Page
    {
        private readonly MesasViewModel _viewModel;

        public MesasPage()
        {
            InitializeComponent();

            _viewModel = App.MesasViewModel;
            DataContext = _viewModel;
        }

        private void btnAgregarMesa_Click(object sender, RoutedEventArgs e)
        {
            string input = Microsoft.VisualBasic.Interaction.InputBox(
                "Ingrese el número de la nueva mesa:",
                "Agregar Mesa",
                ""
            );

            if (!int.TryParse(input, out int numeroMesa))
            {
                MessageBox.Show("Número de mesa con formato incorrecto.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (_viewModel.AgregarMesa(numeroMesa))
                MessageBox.Show("Mesa agregada con éxito.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            if (button?.Tag is MesaWrapper mesa && _viewModel.EliminarMesa(mesa.Mesa))
                _viewModel.CargarMesas();
        }

        private void btnOrdenes_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            bool? result;

            OrdenMesaWindow modal;

            if (button?.Tag is MesaWrapper mesa)
            {
                var ordenExistente = _viewModel.ExisteOrden(mesa.IdMesa);

                if (ordenExistente != null)
                {
                    modal = new OrdenMesaWindow(ordenExistente);
                    result = modal.ShowDialog();
                    return;
                }

                var confirmar = MessageBox.Show($"¿Quieres abrir una orden para la mesa #{mesa.Numero}?",
                    "Abrir Orden", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (confirmar != MessageBoxResult.Yes)
                    return;

                var orden = _viewModel.ActivarMesa(mesa);

                if (orden == null)
                {
                    MessageBox.Show("Ocurrió un error al abrir la orden.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                modal = new OrdenMesaWindow(orden);

                result = modal.ShowDialog();
            }
        }
    }
}
