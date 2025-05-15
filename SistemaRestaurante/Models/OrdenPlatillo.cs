using System;
using System.Collections.Generic;

namespace SistemaRestaurante.Models;

public partial class OrdenPlatillo
{
    public int IdOrdenPlatillo { get; set; }

    public int OrdenId { get; set; }

    public int PlatilloId { get; set; }

    public int Cantidad { get; set; }

    public bool Estatus { get; set; }

    public virtual Orden Orden { get; set; } = null!;

    public virtual Platillo Platillo { get; set; } = null!;
}
