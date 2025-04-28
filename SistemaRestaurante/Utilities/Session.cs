using static SistemaRestaurante.Utilities.GlobalUtilities;

namespace SistemaRestaurante.Utilities
{
    internal class Session
    {
        private static Session? _instance;

        public int? IdUsuario { get; set; }
        public string? Usuario { get; set; }
        public string? Nombre { get; set; }
        public string? Rol { get; set; }

        private Session() { }

        public static Session Instance => _instance ??= new Session();

        public static bool EsAdmin() => Instance.Rol == Roles.Administrador.ToString();
    }
}
