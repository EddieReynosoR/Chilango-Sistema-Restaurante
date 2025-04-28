using System.Globalization;
using System.Windows.Data;

namespace SistemaRestaurante.Converters
{
    internal class BoolToStatusTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? "Ocupada" : "Disponible";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
