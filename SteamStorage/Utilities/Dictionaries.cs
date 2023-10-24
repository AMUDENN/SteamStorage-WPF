using System.Windows;
using System.Windows.Media;

namespace SteamStorage.Utilities
{
    public static class Dictionaries
    {
        public static Style? GetStyle(string style) => Application.Current.Resources[style] as Style;
        public static SolidColorBrush? GetSolidColorBrush(string solidColorBrush) => Application.Current.Resources[solidColorBrush] as SolidColorBrush;
        public static Color? GetColor(string color) => Application.Current.Resources[color] as Color?;
    }
}
