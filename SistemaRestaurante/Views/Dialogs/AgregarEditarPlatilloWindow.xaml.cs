using SistemaRestaurante.Models;
using SistemaRestaurante.Utilities;
using SistemaRestaurante.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SistemaRestaurante.Views.Dialogs
{
    /// <summary>
    /// Interaction logic for AgregarEditarPlatilloWindow.xaml
    /// </summary>
    public partial class AgregarEditarPlatilloWindow : Window
    {
        private readonly AgregarEditarPlatilloViewModel _viewModel;
        public AgregarEditarPlatilloWindow(Platillo? platillo = null)
        {
            InitializeComponent();
            _viewModel = new AgregarEditarPlatilloViewModel(platillo);
            DataContext = _viewModel;
        }

        private void tbxPrecio_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextDecimal(e.Text, ((TextBox)sender).Text);
        }

        private void tbxPrecio_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                string pastedText = (string)e.DataObject.GetData(typeof(string));
                var currentText = ((TextBox)sender).Text;
                if (!IsTextDecimal(pastedText, currentText))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        private bool IsTextDecimal(string input, string currentText)
        {
            string fullText = currentText + input;

            return decimal.TryParse(fullText, out _);
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (!decimal.TryParse(tbxPrecio.Text, out decimal precio))
            {
                MessageBox.Show("El precio no tiene el formato correcto.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string areaSeleccionada = ((ComboBoxItem)cbCategoria.SelectedItem).Content.ToString();

            GlobalUtilities.AreaPlatillo area = areaSeleccionada == GlobalUtilities.AreaPlatillo.Cocina.ToString()
                ? GlobalUtilities.AreaPlatillo.Cocina
                : GlobalUtilities.AreaPlatillo.Bebidas;

            bool exito = _viewModel.GuardarPlatillo(new Platillo
            {
                Nombre = tbxNombre.Text,
                Precio = precio,
                Area = area.ToString(),
                Estatus = true
            });

            if (exito)
                Close();
        }

        private void btnAñadirPlatillo_Click(object sender, RoutedEventArgs e)
        {
            var model = new AgregarIngredientePlatilloWindow(_viewModel.Productos?.Select(p => p.Producto).ToList());

            bool? resultado = model.ShowDialog();

            if (resultado == true)
                _viewModel.CargarDesdeModelo([.. model.ProductosSeleccionadosResultado]);
        }

        private void btnAumentar_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.Tag is ProductoViewModel productoVM && productoVM.StockActual < 10)
                productoVM.StockActual++;
        }

        private void btnDisminuir_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.Tag is ProductoViewModel productoVM && productoVM.StockActual > 1)
                productoVM.StockActual--;
        }
    }
}
