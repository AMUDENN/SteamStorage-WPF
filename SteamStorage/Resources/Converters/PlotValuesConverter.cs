using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SteamStorage.Resources.Converters
{
    public class PlotValueConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null || parameter is null) return value;
            double number = (double)value;
            string command = (string)parameter;
            double result = 0;
            if (command == "*") result = number * 1.2;
            if (command == "/") result = number / 1.2;
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}