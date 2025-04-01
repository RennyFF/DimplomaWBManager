using System.Windows;
using System.Windows.Data;

namespace MYWFE.Utils.Converters
{
    internal class NullToVisibilityByBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Visibility result;
            if (value != null)
            {
                if (parameter != null)
                {
                    if ((bool)value == true)
                    {
                        result = Visibility.Collapsed;
                    }
                    else
                    {
                        result = Visibility.Visible;
                    }
                }
                else
                {
                    if ((bool)value == true)
                    {
                        result = Visibility.Visible;
                    }
                    else
                    {
                        result = Visibility.Collapsed;
                    }
                }
            }
            else result = Visibility.Collapsed;
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
