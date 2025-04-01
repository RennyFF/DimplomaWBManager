using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MYWFE.Utils.Converters
{
    class GreaterThanZeroConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int count)
            {
                if (parameter != null) {
                    return count > 0 ? Visibility.Collapsed : Visibility.Visible;
                }
                else return count > 0 ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
