using Microsoft.EntityFrameworkCore;
using SistemaRestaurante.Models;
using SistemaRestaurante.ViewModels;
using System.Collections.ObjectModel;
using static SistemaRestaurante.Utilities.GlobalUtilities;

namespace SistemaRestaurante.Repositories
{
    internal class OrdenRepository
    {
        private readonly RestauranteDbContext _context;

        public OrdenRepository(RestauranteDbContext context)
        {
            _context = context;
        }

        public Orden? ExisteOrdenEnMesa(int idMesa) => _context.Ordens.FirstOrDefault(o => o.MesaId == idMesa && o.Estatus);

        public bool AgregarOrden(Orden orden)
        {
            _context.Ordens.Add(orden);
            return _context.SaveChanges() > 0;
        }

        public List<PlatilloSeleccionado> ObtenerPlatillosGuardados(int idOrden)
        {
            return _context.OrdenPlatillos
                .Where(o => o.OrdenId == idOrden && o.Platillo != null && o.Estatus)
                .Select(o => new PlatilloSeleccionado(o.Platillo)
                {
                    IsSelected = true,
                    Cantidad = o.Cantidad
                })
                .ToList();
        }

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
                {
                    platilloEliminar.Estatus = false;
                    _context.Entry(platilloEliminar).State = EntityState.Modified;
                }

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

        public bool CancelarOrden(int idOrden)
        {
            using var transaction = _context.Database.BeginTransaction();

            try
            {
                var platillosAsignados = _context.OrdenPlatillos
                    .Where(o => o.OrdenId == idOrden)
                    .ToList();

                foreach (var platilloEliminar in platillosAsignados)
                {
                    platilloEliminar.Estatus = false;
                    _context.Entry(platilloEliminar).State = EntityState.Modified;
                }

                var orden = _context.Ordens
                    .FirstOrDefault(o => o.IdOrden == idOrden);

                if (orden != null)
                {
                    var mesa = _context.Mesas.FirstOrDefault(m => m.IdMesa == orden.MesaId);

                    if (mesa != null)
                        mesa.Ocupada = false;

                    orden.Estatus = false;
                    _context.Entry(orden).State = EntityState.Modified;
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

                    if (productoExistente == null || productoExistente.StockActual < producto.CantidadNecesaria * platilloSeleccionado.Cantidad)
                        return false;
                }
            }

            return true;
        }

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
                        {
                            var cantidadARestar = ingrediente.CantidadNecesaria * platilloSeleccionado.Cantidad;

                            if (ingredienteDb.StockActual >= cantidadARestar)
                                ingredienteDb.StockActual -= cantidadARestar;
                            else
                                ingredienteDb.StockActual = 0;
                        }
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
