using System;
using System.Collections.Generic;

namespace SistemaRestaurante.Models;

public partial class Usuario
{
    public int IdUsuario { get; set; }

    public string NombreUsuario { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int IdRol { get; set; }

    public bool Estatus { get; set; }

    public virtual Rol IdRolNavigation { get; set; } = null!;

    public virtual ICollection<Orden> Ordens { get; set; } = new List<Orden>();
}
