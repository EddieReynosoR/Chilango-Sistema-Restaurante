using SistemaRestaurante.Utilities;
using SistemaRestaurante.ViewModels;
using System.Windows;

namespace SistemaRestaurante.Views
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class LoginView : Window
    {
        private readonly LoginViewModel _viewModel;

        public LoginView()
        {
            InitializeComponent();
            _viewModel = new LoginViewModel();
            DataContext = _viewModel;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.UsuarioLogin())
            {
                MessageBox.Show("¡Login exitoso!", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

                GlobalUtilities.OpenWindow(new MainWindow(), this);
            }
            else
                MessageBox.Show("Usuario o contraseña incorrectos", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Módulo no disponible", "Aviso", MessageBoxButton.OK, MessageBoxImage.Information);
            // GlobalUtilities.OpenWindow(new RegistroUsuarioView(), this);
        }
    }
}
