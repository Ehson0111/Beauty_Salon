using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace Beauty_Salon.Converter
{
    public class DiscountBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double discount && discount > 0)
            {
                return new SolidColorBrush(Colors.LightGreen); // Светло-зеленый фон для услуг со скидкой
            }
            return Brushes.Transparent; // Без фона, если скидки нет
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
