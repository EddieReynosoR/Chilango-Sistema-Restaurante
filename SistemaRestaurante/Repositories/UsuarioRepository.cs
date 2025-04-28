using Microsoft.EntityFrameworkCore;
using SistemaRestaurante.Models;
namespace SistemaRestaurante.Repositories
{
    internal class UsuarioRepository
    {
        private readonly RestauranteDbContext _context;

        public UsuarioRepository(RestauranteDbContext context)
        {
            _context = context;
        }

        public Usuario? UsuarioLogin(string nombreUsuario, string password)
        {
            var usuario = _context.Usuarios
                                    .Include(u => u.IdRolNavigation)
                                    .FirstOrDefault(u => u.NombreUsuario == nombreUsuario
                                        && u.Password == password);

            return usuario;
        }
    }
}
