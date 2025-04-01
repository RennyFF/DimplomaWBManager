using System.Windows.Data;
using System.Windows;

namespace MYWFE.Utils.Converters
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Visibility result;
            if (parameter != null)
            {
                result = Visibility.Collapsed;
                if (value is bool)
                {
                    if (!(bool)value)
                    {
                        result = Visibility.Visible;
                    }
                }
            }
            else
            {
                result = Visibility.Visible;
                if (value is bool)
                {
                    if (!(bool)value)
                    {
                        result = Visibility.Collapsed;
                    }
                }
            }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool result = true;
            if (value is Visibility)
            {
                if ((Visibility)value == Visibility.Hidden)
                {
                    result = false;
                }
            }
            return result;
        }
    }
}
