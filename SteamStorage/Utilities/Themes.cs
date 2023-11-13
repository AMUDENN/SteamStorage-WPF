using SteamStorage.Services.Config;
using SteamStorage.Services.Logger;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace SteamStorage.Utilities
{
    public static class Themes
    {
        #region Constants
        public const string DefaultMainColor = "7371FF";
        public const string DefaultMainAdditionalColor = "00AD71";
        public const string DefaultAdditionalColor = "FFFFFF";
        public const string DefaultAccentColor = "0D1117";
        public const string DefaultAccentAdditionalColor = "21262D";
        public const string DefaultPercentPlusColor = "02B478";
        public const string DefaultPercentMinusColor = "FD4534";
        #endregion Constants

        #region Enums
        public enum ThemesEnum { Light, Dark, Custom };
        #endregion Enums

        #region Fields
        private static readonly Dictionary<ThemesEnum, string> _themesPath = new()
        {
            { ThemesEnum.Light, @"Resources\Themes\LightTheme.xaml" },
            { ThemesEnum.Dark, @"Resources\Themes\DarkTheme.xaml" }
        };
        private static readonly ConfigService? _configService = Singleton.GetService<ConfigService>();
        private static readonly LoggerService? _loggerService = Singleton.GetService<LoggerService>();
        #endregion Fields

        #region Methods
        public static void ChangeTheme(ThemesEnum theme)
        {
            _configService.CurrentTheme = theme;

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
            try
            {
                resources["Main"] = _configService.MainColor;
                resources["MainAdditional"] = _configService.MainAdditionalColor;
                resources["Additional"] = _configService.AdditionalColor;
                resources["Accent"] = _configService.AccentColor;
                resources["AccentAdditional"] = _configService.AccentAdditionalColor;
                resources["PercentPlus"] = _configService.PercentPlusColor;
                resources["PercentMinus"] = _configService.PercentMinusColor;
            }
            catch (Exception ex)
            {
                resources["Main"] = GetSolidColorBrush(DefaultMainColor);
                resources["MainAdditional"] = GetSolidColorBrush(DefaultMainAdditionalColor);
                resources["Additional"] = GetSolidColorBrush(DefaultAdditionalColor);
                resources["Accent"] = GetSolidColorBrush(DefaultAccentColor);
                resources["AccentAdditional"] = GetSolidColorBrush(DefaultAccentAdditionalColor);
                resources["PercentPlus"] = GetSolidColorBrush(DefaultPercentPlusColor);
                resources["PercentMinus"] = GetSolidColorBrush(DefaultPercentMinusColor);
                _loggerService?.WriteMessage(ex, "Установка пользовательских цветов прошла неудачно!");
            }
        }
        public static SolidColorBrush? GetSolidColorBrush(string color)
        {
            var converter = new BrushConverter();
            return converter.ConvertFrom($"#{color}") as SolidColorBrush;
        }
        #endregion Methods
    }
}
