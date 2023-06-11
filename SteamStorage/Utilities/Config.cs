using System;
using System.Configuration;

namespace SteamStorage.Utilities
{
    public class Config
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
        private static void SaveProperty(string name, string value)
        {
            config.AppSettings.Settings[name].Value = value;
            config.Save();
            ConfigurationManager.RefreshSection("appSettings");
        }
    }
}
