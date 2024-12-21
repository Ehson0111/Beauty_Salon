using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Beauty_Salon.Converter
{
    public class DiscountToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Проверяем, является ли значение скидки допустимым и больше нуля
            if (value is double discount) // или decimal, в зависимости от типа
            {
                return discount > 0 ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed; // Если скидка - null или неподходящее значение
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
