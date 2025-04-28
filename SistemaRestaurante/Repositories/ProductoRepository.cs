using Microsoft.EntityFrameworkCore;
using SistemaRestaurante.Models;

namespace SistemaRestaurante.Repositories
{
    /// <summary>
    /// Repositorio para gestionar operaciones relacionadas con productos en el restaurante.
    /// </summary>
    internal class ProductoRepository
    {
        private readonly RestauranteDbContext _context;

        /// <summary>
        /// Inicializa una nueva instancia del repositorio de productos.
        /// </summary>
        /// <param name="context">El contexto de base de datos del restaurante.</param>
        public ProductoRepository(RestauranteDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtiene todos los productos registrados en la base de datos.
        /// </summary>
        /// <returns>Una lista de productos.</returns>
        public List<Producto> ObtenerProductos() => [.. _context.Productos];

        /// <summary>
        /// Agrega un nuevo producto a la base de datos.
        /// </summary>
        /// <param name="producto">El producto a agregar.</param>
        /// <returns>True si el producto se guardó correctamente; de lo contrario, False.</returns>
        public bool AgregarProducto(Producto producto)
        {
            _context.Productos.Add(producto);
            return _context.SaveChanges() > 0;
        }

        /// <summary>
        /// Obtiene un producto específico mediante su ID.
        /// </summary>
        /// <param name="idProducto">El identificador del producto.</param>
        /// <returns>El producto encontrado o null si no existe.</returns>
        public Producto? ObtenerProductoPorId(int idProducto) => _context.Productos.FirstOrDefault(p => p.IdProducto == idProducto);

        /// <summary>
        /// Edita la información de un producto existente.
        /// </summary>
        /// <param name="producto">El producto modificado.</param>
        /// <returns>True si se guardaron los cambios; de lo contrario, False.</returns>
        public bool EditarProducto(Producto producto)
        {
            _context.Productos.Attach(producto);
            _context.Entry(producto).State = EntityState.Modified;
            return _context.SaveChanges() > 0;
        }

        /// <summary>
        /// Elimina un producto de la base de datos.
        /// </summary>
        /// <param name="producto">El producto a eliminar.</param>
        /// <returns>True si el producto se eliminó correctamente; de lo contrario, False.</returns>
        public bool EliminarProducto(Producto producto)
        {
            _context.Productos.Remove(producto);
            return _context.SaveChanges() > 0;
        }

        /// <summary>
        /// Agrega un nuevo producto que será utilizado en un platillo.
        /// </summary>
        /// <param name="producto">El producto a agregar.</param>
        /// <returns>True si el producto se guardó correctamente; de lo contrario, False.</returns>
        public bool AgregarProductoEnPlatillo(Producto producto)
        {
            _context.Productos.Add(producto);
            return _context.SaveChanges() > 0;
        }

        /// <summary>
        /// Obtiene la lista de productos que necesitan reabastecimiento, junto con la cantidad faltante.
        /// </summary>
        /// <returns>Una lista de tuplas que contiene el producto y la cantidad faltante para alcanzar el mínimo.</returns>
        public List<(Producto Producto, int CantidadFaltante)> ObtenerProductosParaReabastecer()
        {
            return [.. _context.Productos
                .Where(p => p.StockActual < p.Minimo)
                .AsEnumerable()
                .Select(p => (p, p.Minimo - p.StockActual))];
        }
    }
}
