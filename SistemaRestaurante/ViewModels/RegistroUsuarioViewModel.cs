using SistemaRestaurante.Models;
using SistemaRestaurante.Repositories;
using SistemaRestaurante.Utilities;
using System.Collections.ObjectModel;
using System.Windows;

namespace SistemaRestaurante.ViewModels
{
    internal class RegistroUsuarioViewModel : ViewModelBase
    {
        private string _username;
        private string _nombre;
        private string _password;

        private Rol _rolSeleccionado;

        private readonly UsuarioRepository _usuarioRepository;

        public string Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(nameof(Username)); }
        }

        public string Nombre
        {
            get => _nombre;
            set { _nombre = value; OnPropertyChanged(nameof(Nombre)); }
        }

        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(nameof(Password)); }
        }

        public ObservableCollection<Rol> Roles { get; set; }

        public Rol RolSeleccionado
        {
            get => _rolSeleccionado;
            set
            {
                _rolSeleccionado = value;
                OnPropertyChanged(nameof(RolSeleccionado));
                RolIdSeleccionado = _rolSeleccionado?.IdRol ?? 0;
            }
        }

        public int RolIdSeleccionado { get; set; }

        public RegistroUsuarioViewModel()
        {
            _usuarioRepository = new UsuarioRepository(new SoftwareRestauranteContext());
            ConsultarRoles();
        }

        public bool ConsultarRoles()
        {
            try
            {
                var roles = _usuarioRepository.ConsultarRoles();

                if (roles == null || roles.Count == 0)
                {
                    MessageBox.Show("No se encontraron roles registrados.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                    return false;
                }

                Roles = [.. roles];
                return true;
            }
            catch(Exception e)
            {
                MessageBox.Show($"Ocurrió un error al asignar los platillos a la orden. {e.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                return false;
            }
        }

        public bool RegistrarUsuario()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Nombre) || string.IsNullOrWhiteSpace(Password) || RolIdSeleccionado == 0)
                {
                    MessageBox.Show("Por favor, complete todos los campos.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }

                var hashedPassword = PasswordHasher.Hash(Password);

                var registro = _usuarioRepository.RegistrarUsuario(new Usuario
                {
                    NombreUsuario = Username,
                    Nombre = Nombre,
                    Password = hashedPassword,
                    IdRol = RolIdSeleccionado,
                    Estatus = true
                });

                if (!registro)
                {
                    MessageBox.Show("Ocurrió un error al registrar al nuevo usuario.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                    return false;
                }

                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show($"Ocurrió un error al registrar el usuario. {e.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                return false;
            }
        }
    }
}
