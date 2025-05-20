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

        public Usuario? UsuarioLogin(string nombreUsuario)
        {
            return _context.Usuarios
                .Include(u => u.IdRolNavigation)
                .FirstOrDefault(u => u.NombreUsuario == nombreUsuario
                    && u.Estatus);
        }

        public List<Rol> ConsultarRoles() => [.. _context.Rols.Where(r => r.Estatus)];

        public bool RegistrarUsuario(Usuario usuario)
        {
            _context.Usuarios.Add(usuario);
            return _context.SaveChanges() > 0;
        }
    }
}
