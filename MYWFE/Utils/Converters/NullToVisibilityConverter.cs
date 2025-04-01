using System.Windows;
using System.Windows.Data;

namespace MYWFE.Utils.Converters
{
    internal class NullToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Visibility result;
            if (parameter != null)
            {
                result = Visibility.Collapsed;
                if (value is object)
                {
                    if (value != null)
                    {
                        result = Visibility.Visible;
                    }
                }
            }
            else
            {
                result = Visibility.Visible;
                if (value is object)
                {
                    if (value != null)
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
