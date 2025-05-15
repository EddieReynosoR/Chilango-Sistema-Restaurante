using Microsoft.EntityFrameworkCore;
using SistemaRestaurante.Models;

namespace SistemaRestaurante.Repositories
{
    /// <summary>
    /// Repositorio para manejar operaciones CRUD relacionadas con las mesas del restaurante.
    /// </summary>
    internal class MesaRepository
    {
        private readonly RestauranteDbContext _context;

        public MesaRepository(RestauranteDbContext context)
        {
            _context = context;
        }

        public List<Mesa> ObtenerMesas() => [.. _context.Mesas.Where(m => m.Estatus)];

        public bool GuardarCambios(Mesa mesa)
        {
            _context.Entry(mesa).State = EntityState.Modified;
            return _context.SaveChanges() > 0;
        }

        public bool AgregarMesa(int numeroMesa)
        {
            _context.Mesas.Add(new Mesa
            {
                Numero = numeroMesa,
                Ocupada = false
            });
            return _context.SaveChanges() > 0;
        }
    }
}
