using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace SteamStorage.Utilities
{
    public class Themes
    {
        public enum ThemesEnum { Light, Dark, Custom };
        private static Dictionary<ThemesEnum, string> themesPath = new Dictionary<ThemesEnum, string>()
        {
            { ThemesEnum.Light, @"Resources\Themes\LightTheme.xaml" },
            { ThemesEnum.Dark, @"Resources\Themes\DarkTheme.xaml" }
        };
        public static void ChangeTheme(ThemesEnum theme)
        {
            Config.CurrentTheme = theme.ToString();

            if(theme == ThemesEnum.Custom)
            {
                SetCustomColors();
                return;
            }

            var uri = new Uri(themesPath[theme], UriKind.Relative);
            ResourceDictionary resourceDict = Application.LoadComponent(uri) as ResourceDictionary;
            Application.Current.Resources.Clear();
            Application.Current.Resources.MergedDictionaries.Add(resourceDict);
        }
        public static void SetCustomColors()
        {
            var resources = Application.Current.Resources;
            var converter = new BrushConverter();
            resources["Main"] = (SolidColorBrush)converter.ConvertFrom(Config.MainColor);
            resources["MainAdditional"] = (SolidColorBrush)converter.ConvertFrom(Config.MainAdditionalColor);
            resources["Additional"] = (SolidColorBrush)converter.ConvertFrom(Config.AdditionalColor);
            resources["Accent"] = (SolidColorBrush)converter.ConvertFrom(Config.AccentColor);
            resources["AccentAdditional"] = (SolidColorBrush)converter.ConvertFrom(Config.AccentAdditionalColor);
            resources["PercentPlus"] = (SolidColorBrush)converter.ConvertFrom(Config.PercentPlusColor);
            resources["PercentMinus"] = (SolidColorBrush)converter.ConvertFrom(Config.PercentMinusColor);
        }
    }
}
