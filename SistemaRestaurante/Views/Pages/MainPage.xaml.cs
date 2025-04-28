using SistemaRestaurante.ViewModels;
using System.Windows.Controls;

namespace SistemaRestaurante.Views.Pages
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }
    }
}
