﻿using System.Windows;
using System.Windows.Media;

namespace SteamStorage.Utilities
{
    public class Dictionaries
    {
        public static Style GetStyle(string style) => (Style)Application.Current.Resources[style];
        public static SolidColorBrush GetSolidColorBrush(string solidColorBrush) => (SolidColorBrush)Application.Current.Resources[solidColorBrush];
    }
}
