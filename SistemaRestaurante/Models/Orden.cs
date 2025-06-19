using System;
using System.Collections.Generic;

namespace SistemaRestaurante.Models;

public partial class Orden
{
    public int IdOrden { get; set; }

    public int? MesaId { get; set; }

    public DateTime Fecha { get; set; }

    public int? IdUsuario { get; set; }

    public bool Estatus { get; set; }

    public bool EsTardio { get; set; }

    public int SegundosTardio { get; set; }

    public virtual Usuario? IdUsuarioNavigation { get; set; }

    public virtual Mesa? Mesa { get; set; }

    public virtual ICollection<OrdenPlatillo> OrdenPlatillos { get; set; } = new List<OrdenPlatillo>();

    public virtual ICollection<Ventum> Venta { get; set; } = new List<Ventum>();
}
