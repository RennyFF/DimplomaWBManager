using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace MYWFE.Utils.Converters
{
    public class IndexOfListboxToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ListBoxItem item)
            {
                var listBox = ItemsControl.ItemsControlFromItemContainer(item) as ListBox;
                if (listBox != null)
                {
                    int index = listBox.ItemContainerGenerator.IndexFromContainer(item);
                    return $"Изображение {index + 1}";
                }
            }
            return "Изображение";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
