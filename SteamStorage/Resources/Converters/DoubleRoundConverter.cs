using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SteamStorage.Resources.Converters
{
    public class DoubleRoundConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null || parameter is null) return value;
            double number = (double)value;
            int param = System.Convert.ToInt32(parameter);
            return Math.Round(number, param);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}