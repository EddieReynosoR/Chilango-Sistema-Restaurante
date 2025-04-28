using SistemaRestaurante.Models;
using SistemaRestaurante.ViewModels;
using System.Collections.ObjectModel;
using static SistemaRestaurante.Utilities.GlobalUtilities;

namespace SistemaRestaurante.Repositories
{
    /// <summary>
    /// Repositorio para manejar las operaciones relacionadas con las órdenes del restaurante.
    /// </summary>
    internal class OrdenRepository
    {
        private readonly RestauranteDbContext _context;

        /// <summary>
        /// Inicializa una nueva instancia del repositorio de órdenes.
        /// </summary>
        /// <param name="context">Contexto de base de datos de restaurante.</param>
        public OrdenRepository(RestauranteDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Verifica si existe una orden activa en una mesa específica.
        /// </summary>
        /// <param name="idMesa">Identificador de la mesa.</param>
        /// <returns>La orden activa si existe; de lo contrario, <c>null</c>.</returns>
        public Orden? ExisteOrdenEnMesa(int idMesa) => _context.Ordens.FirstOrDefault(o => o.MesaId == idMesa && o.Estatus);

        /// <summary>
        /// Agrega una nueva orden al sistema.
        /// </summary>
        /// <param name="orden">Orden a agregar.</param>
        /// <returns><c>true</c> si la operación fue exitosa; de lo contrario, <c>false</c>.</returns>
        public bool AgregarOrden(Orden orden)
        {
            _context.Ordens.Add(orden);
            return _context.SaveChanges() > 0;
        }

        /// <summary>
        /// Obtiene los platillos previamente guardados en una orden.
        /// </summary>
        /// <param name="idOrden">Identificador de la orden.</param>
        /// <returns>Lista de platillos seleccionados.</returns>
        public List<PlatilloSeleccionado> ObtenerPlatillosGuardados(int idOrden)
        {
            return _context.OrdenPlatillos
                .Where(o => o.OrdenId == idOrden && o.Platillo != null)
                .Select(o => new PlatilloSeleccionado(o.Platillo)
                {
                    IsSelected = true,
                    Cantidad = o.Cantidad
                })
                .ToList();
        }

        /// <summary>
        /// Guarda los platillos seleccionados para una orden específica.
        /// </summary>
        /// <param name="platillos">Colección de platillos seleccionados.</param>
        /// <param name="idOrden">Identificador de la orden.</param>
        /// <returns><c>true</c> si la operación fue exitosa; de lo contrario, <c>false</c>.</returns>
        public bool GuardarPlatillosOrden(ObservableCollection<PlatilloSeleccionado> platillos, int idOrden)
        {
            using var transaction = _context.Database.BeginTransaction();

            try
            {
                var platillosExistentes = _context.OrdenPlatillos
                    .Where(op => op.OrdenId == idOrden)
                    .ToList();

                var platillosParaEliminar = platillosExistentes
                    .Where(op => !platillos.Any(p => p.Platillo.IdPlatillo == op.PlatilloId))
                    .ToList();

                foreach (var platilloEliminar in platillosParaEliminar)
                    _context.OrdenPlatillos.Remove(platilloEliminar);

                foreach (var platillo in platillos)
                {
                    var platilloExistente = platillosExistentes
                        .FirstOrDefault(op => op.PlatilloId == platillo.Platillo.IdPlatillo);

                    if (platilloExistente != null)
                    {
                        platilloExistente.Cantidad = platillo.Cantidad;
                        continue;
                    }

                    _context.OrdenPlatillos.Add(new OrdenPlatillo
                    {
                        PlatilloId = platillo.Platillo.IdPlatillo,
                        OrdenId = idOrden,
                        Cantidad = platillo.Cantidad
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

        /// <summary>
        /// Cancela una orden existente y libera la mesa asociada.
        /// </summary>
        /// <param name="idOrden">Identificador de la orden a cancelar.</param>
        /// <returns><c>true</c> si la cancelación fue exitosa; de lo contrario, <c>false</c>.</returns>
        public bool CancelarOrden(int idOrden)
        {
            using var transaction = _context.Database.BeginTransaction();

            try
            {
                var platillosAsignados = _context.OrdenPlatillos
                    .Where(o => o.OrdenId == idOrden)
                    .ToList();

                _context.OrdenPlatillos.RemoveRange(platillosAsignados);

                var orden = _context.Ordens
                    .FirstOrDefault(o => o.IdOrden == idOrden);

                if (orden != null)
                {
                    var mesa = _context.Mesas.FirstOrDefault(m => m.IdMesa == orden.MesaId);

                    if (mesa != null)
                        mesa.Ocupada = false;

                    _context.Ordens.Remove(orden);
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

        /// <summary>
        /// Valida que haya suficiente stock de productos para los platillos seleccionados.
        /// </summary>
        /// <param name="platillos">Colección de platillos seleccionados.</param>
        /// <returns><c>true</c> si hay suficiente stock para todos los productos; de lo contrario, <c>false</c>.</returns>
        public bool ValidarCantidadProductos(ObservableCollection<PlatilloSeleccionado> platillos)
        {
            foreach (var platilloSeleccionado in platillos)
            {
                var productos = _context.PlatilloProductos
                    .Where(pi => pi.PlatilloId == platilloSeleccionado.Platillo.IdPlatillo)
                    .ToList();

                foreach (var producto in productos)
                {
                    var productoExistente = _context.Productos.FirstOrDefault(p => p.IdProducto == producto.ProductoId);

                    if (productoExistente == null || productoExistente.StockActual < producto.CantidadNecesaria)
                        return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Genera una venta a partir de una orden, actualizando inventarios y registrando propinas.
        /// </summary>
        /// <param name="platillos">Colección de platillos vendidos.</param>
        /// <param name="idOrden">Identificador de la orden.</param>
        /// <param name="propina">Porcentaje de propina aplicado.</param>
        /// <returns><c>true</c> si la venta se registró exitosamente; de lo contrario, <c>false</c>.</returns>
        public bool GenerarVenta(ObservableCollection<PlatilloSeleccionado> platillos, int idOrden, decimal propina)
        {
            using var transaction = _context.Database.BeginTransaction();

            try
            {
                var orden = _context.Ordens.FirstOrDefault(o => o.IdOrden == idOrden);

                if (orden == null || orden.MesaId == null)
                    return false;

                int idMesa = (int)orden.MesaId;
                var mesa = _context.Mesas.FirstOrDefault(m => m.IdMesa == idMesa);

                if (mesa == null)
                    return false;

                foreach (var platilloSeleccionado in platillos)
                {
                    var ingredientes = _context.PlatilloProductos
                        .Where(pi => pi.PlatilloId == platilloSeleccionado.Platillo.IdPlatillo)
                        .ToList();

                    foreach (var ingrediente in ingredientes)
                    {
                        var ingredienteDb = _context.Productos.FirstOrDefault(i => i.IdProducto == ingrediente.ProductoId);

                        if (ingredienteDb != null)
                            ingredienteDb.StockActual -= ingrediente.CantidadNecesaria;
                    }
                }

                decimal propinaBebidas = 0;
                decimal propinaCocina = 0;
                decimal propinaMesero = 0;

                decimal total = platillos.Sum(p => p.Platillo.Precio * p.Cantidad);
                decimal propinaOrden = total * propina;

                bool aplicaCocina = platillos.Any(p => p.Platillo.Area == AreaPlatillo.Cocina.ToString());
                bool aplicaBebidas = platillos.Any(p => p.Platillo.Area == AreaPlatillo.Bebidas.ToString());

                if (aplicaCocina)
                    propinaCocina = propinaOrden * (decimal)CantidadPropina[Propinas.Cocina];

                if (aplicaBebidas)
                    propinaBebidas = propinaOrden * (decimal)CantidadPropina[Propinas.Bebidas];

                if (!aplicaBebidas && aplicaCocina)
                    propinaCocina += propinaOrden * (decimal)CantidadPropina[Propinas.Bebidas];

                if (!aplicaCocina && aplicaBebidas)
                    propinaBebidas += propinaOrden * (decimal)CantidadPropina[Propinas.Cocina];

                propinaMesero = propinaOrden * (decimal)CantidadPropina[Propinas.Mesero];

                orden.Estatus = false;
                mesa.Ocupada = false;

                var venta = new Ventum
                {
                    IdOrden = orden.IdOrden,
                    Total = total,
                    PropinaMesero = propinaMesero,
                    PropinaCocina = propinaCocina,
                    PropinaBebidas = propinaBebidas,
                    FechaVenta = DateTime.Now
                };

                _context.Venta.Add(venta);

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
