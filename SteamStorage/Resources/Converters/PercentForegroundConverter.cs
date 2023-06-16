using SteamStorage.Utilities;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace SteamStorage.Resources.Converters
{
    public class PercentForegroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null) return value;
            double percent = (double)value;
            SolidColorBrush solidColorBrush = Dictionaries.GetSolidColorBrush(percent < 0 ? "PercentMinus" : "PercentPlus");
            if (parameter is not null && ((string)parameter) == "Color") return solidColorBrush.Color;
            return solidColorBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}