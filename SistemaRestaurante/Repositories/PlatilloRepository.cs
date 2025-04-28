using SistemaRestaurante.Models;
using static SistemaRestaurante.Utilities.GlobalUtilities;

namespace SistemaRestaurante.Repositories
{
    /// <summary>
    /// Repositorio para operaciones relacionadas con los platillos del restaurante.
    /// </summary>
    internal class PlatilloRepository
    {
        private readonly RestauranteDbContext _context;

        /// <summary>
        /// Inicializa una nueva instancia del repositorio de platillos.
        /// </summary>
        /// <param name="context">El contexto de base de datos del restaurante.</param>
        public PlatilloRepository(RestauranteDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtiene todos los platillos registrados en la base de datos.
        /// </summary>
        /// <returns>Una lista de platillos.</returns>
        public List<Platillo> ObtenerPlatillos() => [.. _context.Platillos];

        /// <summary>
        /// Obtiene los platillos filtrados por área (Cocina o Bebidas).
        /// </summary>
        /// <param name="area">Área de los platillos a filtrar.</param>
        /// <returns>Una lista de platillos del área especificada.</returns>
        public List<Platillo> ObtenerPlatillos(AreaPlatillo area) =>
            [.. _context.Platillos.Where(p => p.Area == area.ToString())];

        /// <summary>
        /// Obtiene los ingredientes necesarios para preparar un platillo específico.
        /// </summary>
        /// <param name="idPlatillo">Identificador del platillo.</param>
        /// <returns>Una lista de productos que representan los ingredientes del platillo.</returns>
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

        /// <summary>
        /// Guarda un nuevo platillo junto con sus ingredientes en la base de datos.
        /// </summary>
        /// <param name="platillo">El platillo a guardar.</param>
        /// <param name="ingredientes">La lista de ingredientes asociados al platillo.</param>
        /// <returns>True si se guardó correctamente; de lo contrario, False.</returns>
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
    }
}
