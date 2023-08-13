using System;
using System.Configuration;

namespace SteamStorage.Utilities
{
    public static class Config
    {
        private static readonly Configuration _config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        public static string CurrentTheme
        {
            get => _config.AppSettings.Settings["CurrentTheme"].Value;
            set => SaveProperty("CurrentTheme", value);
        }
        public static double Width
        {
            get => Convert.ToDouble(_config.AppSettings.Settings["Width"].Value);
            set => SaveProperty("Width", value.ToString());
        }
        public static double Height
        {
            get => Convert.ToDouble(_config.AppSettings.Settings["Height"].Value);
            set => SaveProperty("Height", value.ToString());
        }
        public static double Top
        {
            get => Convert.ToDouble(_config.AppSettings.Settings["Top"].Value);
            set => SaveProperty("Top", value.ToString());
        }
        public static double Left
        {
            get => Convert.ToDouble(_config.AppSettings.Settings["Left"].Value);
            set => SaveProperty("Left", value.ToString());
        }
        public static bool IsMaximized
        {
            get => Convert.ToBoolean(_config.AppSettings.Settings["IsMaximized"].Value);
            set => SaveProperty("IsMaximized", value.ToString());
        }
        public static string MainColor
        {
            get => _config.AppSettings.Settings["MainColor"].Value;
            set => SaveProperty("MainColor", value);
        }
        public static string MainAdditionalColor
        {
            get => _config.AppSettings.Settings["MainAdditionalColor"].Value;
            set => SaveProperty("MainAdditionalColor", value);
        }
        public static string AdditionalColor
        {
            get => _config.AppSettings.Settings["AdditionalColor"].Value;
            set => SaveProperty("AdditionalColor", value);
        }
        public static string AccentColor
        {
            get => _config.AppSettings.Settings["AccentColor"].Value;
            set => SaveProperty("AccentColor", value);
        }
        public static string AccentAdditionalColor
        {
            get => _config.AppSettings.Settings["AccentAdditionalColor"].Value;
            set => SaveProperty("AccentAdditionalColor", value);
        }
        public static string PercentPlusColor
        {
            get => _config.AppSettings.Settings["PercentPlusColor"].Value;
            set => SaveProperty("PercentPlusColor", value);
        }
        public static string PercentMinusColor
        {
            get => _config.AppSettings.Settings["PercentMinusColor"].Value;
            set => SaveProperty("PercentMinusColor", value);
        }
        public static bool IsGreetingTextVisible
        {
            get => Convert.ToBoolean(_config.AppSettings.Settings["IsGreetingTextVisible"].Value);
            set => SaveProperty("IsGreetingTextVisible", value.ToString());
        }
        private static void SaveProperty(string name, string value)
        {
            _config.AppSettings.Settings[name].Value = value;
            _config.Save();
            ConfigurationManager.RefreshSection("appSettings");
        }
    }
}
