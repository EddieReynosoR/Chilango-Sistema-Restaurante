using SistemaRestaurante.Utilities;
using SistemaRestaurante.ViewModels;
using System.Windows;

namespace SistemaRestaurante.Views
{
    /// <summary>
    /// Interaction logic for RegistroUsuario.xaml
    /// </summary>
    public partial class RegistroUsuarioView : Window
    {
        private readonly RegistroUsuarioViewModel _viewModel;

        public RegistroUsuarioView()
        {
            InitializeComponent();
            _viewModel = new RegistroUsuarioViewModel();
            DataContext = _viewModel;
        }

        private void tbxPassword_PasswordChanged(object sender, RoutedEventArgs e) => _viewModel.Password = tbxPassword.Password;

        private void btnCancelar_Click(object sender, RoutedEventArgs e) => Close();

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            var registro = _viewModel.RegistrarUsuario();

            if (registro)
            {
                MessageBox.Show("¡Registro exitoso!", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

                GlobalUtilities.OpenWindow(new LoginView(), this);
            }
        }
    }
}
