using System;
using System.Collections.Generic;

namespace SistemaRestaurante.Models;

public partial class Mesa
{
    public int IdMesa { get; set; }

    public int Numero { get; set; }

    public bool Ocupada { get; set; }

    public bool Estatus { get; set; }

    public virtual ICollection<Orden> Ordens { get; set; } = new List<Orden>();
}
