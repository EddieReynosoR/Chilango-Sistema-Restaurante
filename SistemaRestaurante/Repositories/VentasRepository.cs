using Microsoft.EntityFrameworkCore;
using SistemaRestaurante.Models;

namespace SistemaRestaurante.Repositories
{
    class VentasRepository
    {
        private readonly RestauranteDbContext _context;

        public VentasRepository(RestauranteDbContext context)
        {
            _context = context;
        }

        public List<Ventum> ObtenerVentas()
        {
            return [.. _context.Venta];
        }

        public List<Ventum> ObtenerVentasPorFecha(DateTime fecha)
        {
            return [.. _context.Venta.Where(v => v.FechaVenta.Date == fecha.Date).AsNoTracking()];
        }

        public bool EliminarVenta(Ventum ventum)
        {
            _context.Venta.Remove(ventum);
            return _context.SaveChanges() > 0;
        }
    }
}
