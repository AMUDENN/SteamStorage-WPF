﻿using SteamStorage.Utilities;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TestSystem.Resources.Converters
{
    public class PercentForegroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null) return value;
            double percent = (double)value;
            return Dictionaries.GetSolidColorBrush( percent < 0 ? "PercentMinus" : "PercentPlus");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}