using SistemaRestaurante.Utilities.Observer;
using SistemaRestaurante.ViewModels;
using System.Windows;

namespace SistemaRestaurante
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        internal static MesasViewModel? MesasViewModel { get; private set; }
        internal static TemporizadorOrdenes? TemporizadorOrdenes { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            TemporizadorOrdenes = new TemporizadorOrdenes();
            MesasViewModel = new MesasViewModel(TemporizadorOrdenes);
        }
    }
}