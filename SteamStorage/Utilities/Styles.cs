using System.Windows;

namespace SteamStorage.Utilities
{
    public class Styles
    {
        public static Style GetStyle(string style) => (Style)Application.Current.Resources[style];
    }
}
