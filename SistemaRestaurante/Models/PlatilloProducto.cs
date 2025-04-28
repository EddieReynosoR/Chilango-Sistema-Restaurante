using System;
using System.Collections.Generic;

namespace SistemaRestaurante.Models;

public partial class PlatilloProducto
{
    public int IdPlatilloProducto { get; set; }

    public int? PlatilloId { get; set; }

    public int? ProductoId { get; set; }

    public int CantidadNecesaria { get; set; }

    public virtual Platillo? Platillo { get; set; }

    public virtual Producto? Producto { get; set; }
}
