using System;
using System.Collections.Generic;

namespace SistemaRestaurante.Models;

public partial class Producto
{
    public int IdProducto { get; set; }

    public string Nombre { get; set; } = null!;

    public int StockActual { get; set; }

    public int Minimo { get; set; }

    public bool Estatus { get; set; }

    public virtual ICollection<PlatilloProducto> PlatilloProductos { get; set; } = new List<PlatilloProducto>();
}
