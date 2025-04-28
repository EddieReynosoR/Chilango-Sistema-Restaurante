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

        /// <summary>
        /// Inicializa una nueva instancia del repositorio de mesas con el contexto de base de datos proporcionado.
        /// </summary>
        /// <param name="context">Contexto de base de datos de restaurante.</param>
        public MesaRepository(RestauranteDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtiene todas las mesas registradas en la base de datos.
        /// </summary>
        /// <returns>Lista de objetos <see cref="Mesa"/>.</returns>
        public List<Mesa> ObtenerMesas() => [.. _context.Mesas];

        /// <summary>
        /// Guarda los cambios realizados a una mesa existente.
        /// </summary>
        /// <param name="mesa">Mesa que se desea actualizar.</param>
        /// <returns><c>true</c> si los cambios fueron guardados correctamente; de lo contrario, <c>false</c>.</returns>
        public bool GuardarCambios(Mesa mesa)
        {
            _context.Entry(mesa).State = EntityState.Modified;
            return _context.SaveChanges() > 0;
        }

        /// <summary>
        /// Elimina una mesa de la base de datos.
        /// </summary>
        /// <param name="mesa">Mesa que se desea eliminar.</param>
        /// <returns><c>true</c> si la eliminación fue exitosa; de lo contrario, <c>false</c>.</returns>
        public bool EliminarMesa(Mesa mesa)
        {
            _context.Mesas.Remove(mesa);
            return _context.SaveChanges() > 0;
        }

        /// <summary>
        /// Agrega una nueva mesa al restaurante.
        /// </summary>
        /// <param name="numeroMesa">Número identificador de la nueva mesa.</param>
        /// <returns><c>true</c> si la mesa fue agregada correctamente; de lo contrario, <c>false</c>.</returns>
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
