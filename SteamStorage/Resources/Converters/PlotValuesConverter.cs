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
            if (command == "Max") result = number < 10 ? number < 1 ? 1 : number + 1 : number * 1.1;
            if (command == "Min") result = number < 10 ? number < 1 ? 0 : number - 1 : number / 1.1;
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}