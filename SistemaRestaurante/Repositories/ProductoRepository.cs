using Microsoft.EntityFrameworkCore;
using SistemaRestaurante.Models;

namespace SistemaRestaurante.Repositories
{
    internal class ProductoRepository
    {
        private readonly SoftwareRestauranteContext _context;

        public ProductoRepository(SoftwareRestauranteContext context)
        {
            _context = context;
        }

        public List<Producto> ObtenerProductos() => [.. _context.Productos.Where(p => p.Estatus)];

        public bool AgregarProducto(Producto producto)
        {
            _context.Productos.Add(producto);
            return _context.SaveChanges() > 0;
        }

        public Producto? ObtenerProductoPorId(int idProducto) => _context.Productos.FirstOrDefault(p => p.IdProducto == idProducto && p.Estatus);

        public bool EditarProducto(Producto producto)
        {
            _context.Productos.Attach(producto);
            _context.Entry(producto).State = EntityState.Modified;
            return _context.SaveChanges() > 0;
        }

        public bool EliminarProducto(Producto producto)
        {
            producto.Estatus = false;
            _context.Entry(producto).State = EntityState.Modified;

            return _context.SaveChanges() > 0;
        }

        public bool AgregarProductoEnPlatillo(Producto producto)
        {
            _context.Productos.Add(producto);
            return _context.SaveChanges() > 0;
        }

        public List<(Producto Producto, int CantidadFaltante)> ObtenerProductosParaReabastecer()
        {
            return [.. _context.Productos
                .Where(p => p.StockActual < p.Minimo)
                .AsEnumerable()
                .Select(p => (p, p.Minimo - p.StockActual))];
        }
    }
}
