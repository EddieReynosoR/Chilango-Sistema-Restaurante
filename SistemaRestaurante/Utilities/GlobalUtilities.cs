using System.Windows;

namespace SistemaRestaurante.Utilities
{
    internal static class GlobalUtilities
    {
        public enum AreaPlatillo
        {
            Cocina,
            Bebidas
        }

        public enum Roles
        {
            Administrador,
            Mesero
        }

        public enum Propinas
        {
            Cocina,
            Bebidas,
            Mesero
        }

        public static Dictionary<Propinas, double> CantidadPropina = new()
        {
            { Propinas.Cocina, 0.2 },
            { Propinas.Bebidas, 0.2 },
            { Propinas.Mesero, 0.6 }
        };

        public static void OpenWindow(Window newWindow, Window prevWindow)
        {
            newWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            Application.Current.MainWindow = newWindow;

            newWindow.Show();
            prevWindow.Close();
        }
    }
}
