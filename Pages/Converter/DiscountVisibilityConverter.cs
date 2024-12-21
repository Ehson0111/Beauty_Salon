using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;

namespace Beauty_Salon.Converter
{
    public class DiscountVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double discount && discount > 0)
            {
                return Visibility.Visible; // Показываем итоговую цену только для услуг со скидкой
            }
            return Visibility.Collapsed; // Скрываем итоговую цену, если скидки нет
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
