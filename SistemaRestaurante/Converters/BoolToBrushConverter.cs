using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace SistemaRestaurante.Converters
{
    internal class BoolToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool ocupada = (bool)value;
            return ocupada ? Brushes.Red : Brushes.Green;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
