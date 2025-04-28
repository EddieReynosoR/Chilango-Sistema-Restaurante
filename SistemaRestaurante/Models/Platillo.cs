using System;
using System.Collections.Generic;

namespace SistemaRestaurante.Models;

public partial class Platillo
{
    public int IdPlatillo { get; set; }

    public string Nombre { get; set; } = null!;

    public decimal Precio { get; set; }

    public string Area { get; set; } = null!;

    public virtual ICollection<OrdenPlatillo> OrdenPlatillos { get; set; } = new List<OrdenPlatillo>();

    public virtual ICollection<PlatilloProducto> PlatilloProductos { get; set; } = new List<PlatilloProducto>();
}
