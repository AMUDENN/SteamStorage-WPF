using System;
using System.Configuration;

namespace SteamStorage.Utilities
{
    public static class Config
    {
        private static readonly Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        public static string CurrentTheme
        {
            get => config.AppSettings.Settings["CurrentTheme"].Value;
            set => SaveProperty("CurrentTheme", value);
        }
        public static double Width
        {
            get => Convert.ToDouble(config.AppSettings.Settings["Width"].Value);
            set => SaveProperty("Width", value.ToString());
        }
        public static double Height
        {
            get => Convert.ToDouble(config.AppSettings.Settings["Height"].Value);
            set => SaveProperty("Height", value.ToString());
        }
        public static double Top
        {
            get => Convert.ToDouble(config.AppSettings.Settings["Top"].Value);
            set => SaveProperty("Top", value.ToString());
        }
        public static double Left
        {
            get => Convert.ToDouble(config.AppSettings.Settings["Left"].Value);
            set => SaveProperty("Left", value.ToString());
        }
        public static bool IsMaximized
        {
            get => Convert.ToBoolean(config.AppSettings.Settings["IsMaximized"].Value);
            set => SaveProperty("IsMaximized", value.ToString());
        }
        public static string MainColor
        {
            get => config.AppSettings.Settings["MainColor"].Value;
            set => SaveProperty("MainColor", value);
        }
        public static string MainAdditionalColor
        {
            get => config.AppSettings.Settings["MainAdditionalColor"].Value;
            set => SaveProperty("MainAdditionalColor", value);
        }
        public static string AdditionalColor
        {
            get => config.AppSettings.Settings["AdditionalColor"].Value;
            set => SaveProperty("AdditionalColor", value);
        }
        public static string AccentColor
        {
            get => config.AppSettings.Settings["AccentColor"].Value;
            set => SaveProperty("AccentColor", value);
        }
        public static string AccentAdditionalColor
        {
            get => config.AppSettings.Settings["AccentAdditionalColor"].Value;
            set => SaveProperty("AccentAdditionalColor", value);
        }
        public static string PercentPlusColor
        {
            get => config.AppSettings.Settings["PercentPlusColor"].Value;
            set => SaveProperty("PercentPlusColor", value);
        }
        public static string PercentMinusColor
        {
            get => config.AppSettings.Settings["PercentMinusColor"].Value;
            set => SaveProperty("PercentMinusColor", value);
        }
        public static bool IsGreetingTextVisible
        {
            get => Convert.ToBoolean(config.AppSettings.Settings["IsGreetingTextVisible"].Value);
            set => SaveProperty("IsGreetingTextVisible", value.ToString());
        }
        private static void SaveProperty(string name, string value)
        {
            config.AppSettings.Settings[name].Value = value;
            config.Save();
            ConfigurationManager.RefreshSection("appSettings");
        }
    }
}
