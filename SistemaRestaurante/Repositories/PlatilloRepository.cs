using Microsoft.EntityFrameworkCore;
using SistemaRestaurante.Models;
using static SistemaRestaurante.Utilities.GlobalUtilities;

namespace SistemaRestaurante.Repositories
{
    internal class PlatilloRepository
    {
        private readonly RestauranteDbContext _context;

        public PlatilloRepository(RestauranteDbContext context)
        {
            _context = context;
        }

        public List<Platillo> ObtenerPlatillos() => [.. _context.Platillos.Where(p => p.Estatus)];

        public List<Platillo> ObtenerPlatillos(AreaPlatillo area) =>
            [.. _context.Platillos.Where(p => p.Area == area.ToString() && p.Estatus)];

        public List<Producto> ObtenerIngredientesPlatillo(int idPlatillo)
        {
            return [.. _context.PlatilloProductos
                .Where(p => p.PlatilloId == idPlatillo)
                .Select(p => new Producto
                {
                    IdProducto = p.Producto.IdProducto,
                    Nombre = p.Producto.Nombre,
                    StockActual = p.CantidadNecesaria
                })];
        }

        public bool GuardarPlatilloConIngredientes(Platillo platillo, List<Producto> ingredientes)
        {
            using var transaction = _context.Database.BeginTransaction();

            try
            {
                _context.Platillos.Add(platillo);
                _context.SaveChanges();

                int idPlatillo = platillo.IdPlatillo;

                foreach (var ingrediente in ingredientes)
                {
                    _context.PlatilloProductos.Add(new PlatilloProducto
                    {
                        PlatilloId = idPlatillo,
                        ProductoId = ingrediente.IdProducto,
                        CantidadNecesaria = ingrediente.StockActual
                    });
                }

                _context.SaveChanges();

                transaction.Commit();
                return true;
            }
            catch (Exception)
            {
                transaction.Rollback();
                return false;
            }
        }

        public bool Eliminar(Platillo platillo)
        {
            try
            {
                platillo.Estatus = false;
                _context.Entry(platillo).State = EntityState.Modified;

                return _context.SaveChanges() > 0;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}
