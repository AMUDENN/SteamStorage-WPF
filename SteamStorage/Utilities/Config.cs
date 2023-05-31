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
        private static void SaveProperty(string name, string value)
        {
            config.AppSettings.Settings[name].Value = value;
            config.Save();
            ConfigurationManager.RefreshSection("appSettings");
        }

    }
}
