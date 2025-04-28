using System;
using System.Collections.Generic;
namespace SistemaRestaurante.Models;

public partial class Ventum
{
    public int IdVenta { get; set; }

    public int IdOrden { get; set; }

    public decimal Total { get; set; }

    public decimal PropinaMesero { get; set; }

    public decimal PropinaCocina { get; set; }

    public decimal PropinaBebidas { get; set; }

    public DateTime FechaVenta { get; set; }

    public virtual Orden IdOrdenNavigation { get; set; } = null!;
}
