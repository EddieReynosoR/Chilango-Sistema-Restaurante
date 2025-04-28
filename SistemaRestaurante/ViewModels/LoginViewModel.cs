using SistemaRestaurante.Repositories;
using SistemaRestaurante.Models;
using SistemaRestaurante.Utilities;

namespace SistemaRestaurante.ViewModels
{
    internal class LoginViewModel : ViewModelBase
    {
        private string _username;
        private string _password;

        /* En varias de las clases ViewModel del proyecto se utilizan datos privados
         * para asegurar que estos solo sean utilizados dentro de la respectiva
         * clase, asegurando así que las clases tengan comportamiento y responsabilidad
         * única. */
        private readonly UsuarioRepository _usuarioRepository;

        public string Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(nameof(Username)); }
        }

        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(nameof(Password)); }
        }

        public LoginViewModel()
        {
            _usuarioRepository = new UsuarioRepository(new RestauranteDbContext());
        }

        public bool UsuarioLogin()
        {
            var usuario = _usuarioRepository.UsuarioLogin(Username, Password);

            if (usuario != null)
            {
                Session.Instance.IdUsuario = usuario.IdUsuario;
                Session.Instance.Usuario = usuario.NombreUsuario;
                Session.Instance.Nombre = usuario.Nombre;
                Session.Instance.Rol = usuario.IdRolNavigation.Nombre;

                return true;
            }

            return false;
        }
    }
}
