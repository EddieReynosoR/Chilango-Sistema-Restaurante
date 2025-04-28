using System.Windows.Controls;
using System.Windows;
using SistemaRestaurante.Utilities;

namespace SistemaRestaurante.Views.UserControls
{
    /// <summary>
    /// Interaction logic for NavBarItem.xaml
    /// </summary>
    public partial class NavBar : UserControl
    {
        public event Action HomeClicked;
        public event Action MesasClicked;
        public event Action OrdenesClicked;
        public event Action VentasClicked;
        public event Action PlatillosClicked;
        public event Action InventarioClicked;
        public event Action CerrarSesionClicked;

        public NavBar()
        {
            InitializeComponent();
            HighlightMenuButton(btnHome);

            if (!Session.EsAdmin())
            {
                btnInventario.Visibility = Visibility.Collapsed;
                btnCorte.Visibility = Visibility.Collapsed;
            }
        }

        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            HomeClicked?.Invoke();
            HighlightMenuButton((Button)sender);
        }

        private void btnMesas_Click(object sender, RoutedEventArgs e)
        {
            MesasClicked?.Invoke();
            HighlightMenuButton((Button)sender);
        }       

        private void btnOrdenes_Click(object sender, RoutedEventArgs e)
        {
            OrdenesClicked?.Invoke();
            HighlightMenuButton((Button)sender);
        }

        private void btnVentas_Click(object sender, RoutedEventArgs e)
        {
            VentasClicked?.Invoke();
            HighlightMenuButton((Button)sender);
        }

        private void btnCorte_Click(object sender, RoutedEventArgs e)
        {
            PlatillosClicked?.Invoke();
            HighlightMenuButton((Button)sender);
        }

        private void btnInventario_Click(object sender, RoutedEventArgs e)
        {
            InventarioClicked?.Invoke();
            HighlightMenuButton((Button)sender);
        }

        private void HighlightMenuButton(Button selectedButton)
        {
            foreach (var child in NavBarPanel.Children)
            {
                if (child is Button btn)
                {
                    btn.ClearValue(TagProperty);
                }
            }

            selectedButton.Tag = "Selected";
        }

        private void btnCerrarSesion_Click(object sender, RoutedEventArgs e) => CerrarSesionClicked?.Invoke();
    }
}
