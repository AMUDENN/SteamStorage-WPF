using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SteamStorage.Resources.Converters
{
    public class PercentConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null) return value;
            double percent = (double)value;
            return (percent < 0 ? percent : "+" + percent) + "%";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}