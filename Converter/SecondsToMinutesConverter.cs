using System;
using System.Globalization;
using System.Windows.Data;

namespace Beauty_Salon.Converter
{
    public class SecondsToMinutesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int seconds)
            {
                int minutes = seconds / 60;
                return $"За {minutes} минут";
            }
            return "0 минут";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
