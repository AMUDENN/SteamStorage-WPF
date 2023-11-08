using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using SteamStorage.Services.Config;

namespace SteamStorage.Utilities
{
    public static class Themes
    {
        public enum ThemesEnum { Light, Dark, Custom };
        private static readonly Dictionary<ThemesEnum, string> _themesPath = new()
        {
            { ThemesEnum.Light, @"Resources\Themes\LightTheme.xaml" },
            { ThemesEnum.Dark, @"Resources\Themes\DarkTheme.xaml" }
        };
        private static readonly ConfigService? _configService = Singleton.GetObject<ConfigService>();
        public static void ChangeTheme(ThemesEnum theme)
        {
            _configService.CurrentTheme = theme.ToString();

            if (theme == ThemesEnum.Custom)
            {
                SetCustomColors();
                return;
            }

            var uri = new Uri(_themesPath[theme], UriKind.Relative);
            ResourceDictionary? resourceDict = Application.LoadComponent(uri) as ResourceDictionary;
            Application.Current.Resources.Clear();
            Application.Current.Resources.MergedDictionaries.Add(resourceDict);
        }
        public static void SetCustomColors()
        {
            var resources = Application.Current.Resources;
            var converter = new BrushConverter();
            resources["Main"] = converter.ConvertFrom($"#{_configService.MainColor}") as SolidColorBrush;
            resources["MainAdditional"] = converter.ConvertFrom($"#{_configService.MainAdditionalColor}") as SolidColorBrush;
            resources["Additional"] = converter.ConvertFrom($"#{_configService.AdditionalColor}") as SolidColorBrush;
            resources["Accent"] = converter.ConvertFrom($"#{_configService.AccentColor}") as SolidColorBrush;
            resources["AccentAdditional"] = converter.ConvertFrom($"#{_configService.AccentAdditionalColor}") as SolidColorBrush;
            resources["PercentPlus"] = converter.ConvertFrom($"#{_configService.PercentPlusColor}") as SolidColorBrush;
            resources["PercentMinus"] = converter.ConvertFrom($"#{_configService.PercentMinusColor}") as SolidColorBrush;
        }
    }
}
