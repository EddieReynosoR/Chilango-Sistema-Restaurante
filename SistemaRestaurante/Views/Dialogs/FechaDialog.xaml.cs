using SistemaRestaurante.ViewModels;
using System.Windows;

namespace SistemaRestaurante.Views.Dialogs
{
    /// <summary>
    /// Interaction logic for FechaDialog.xaml
    /// </summary>
    public partial class FechaDialog : Window
    {
        internal VentaViewModel ViewModel { get; set; }

        public FechaDialog()
        {
            InitializeComponent();
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnGenerar_Click(object sender, RoutedEventArgs e)
        {
            if (!DateTime.TryParse(datePicker.SelectedDate.ToString(), out DateTime fecha))
            {
                MessageBox.Show("La fecha no tiene el formato correcto.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (ViewModel.GenerarFormatoCorteCaja(fecha))
            {
                MessageBox.Show("Se ha generado el reporte de forma correcta.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
        }
    }
}
