using System;
using System.Configuration;
using System.Windows.Media;
using static SteamStorage.Utilities.Themes;

namespace SteamStorage.Services.Config
{
    public class ConfigService : IConfigService
    {
        #region Fields
        private readonly Configuration _config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        #endregion Fields

        #region Properties
        public ThemesEnum CurrentTheme
        {
            get => GetEnumProperty<ThemesEnum>(nameof(CurrentTheme));
            set => SetProperty(nameof(CurrentTheme), value);
        }
        public double Width
        {
            get => GetProperty<double>(nameof(Width));
            set => SetProperty(nameof(Width), value);
        }
        public double Height
        {
            get => GetProperty<double>(nameof(Height));
            set => SetProperty(nameof(Height), value);
        }
        public double Top
        {
            get => GetProperty<double>(nameof(Top));
            set => SetProperty(nameof(Top), value);
        }
        public double Left
        {
            get => GetProperty<double>(nameof(Left));
            set => SetProperty(nameof(Left), value);
        }
        public bool IsMaximized
        {
            get => GetProperty<bool>(nameof(IsMaximized));
            set => SetProperty(nameof(IsMaximized), value);
        }
        public SolidColorBrush MainColor
        {
            get => GetSolidColorBrush(nameof(MainColor));
            set => SetProperty(nameof(MainColor), value);
        }
        public SolidColorBrush MainAdditionalColor
        {
            get => GetSolidColorBrush(nameof(MainAdditionalColor));
            set => SetProperty(nameof(MainAdditionalColor), value);
        }
        public SolidColorBrush AdditionalColor
        {
            get => GetSolidColorBrush(nameof(AdditionalColor));
            set => SetProperty(nameof(AdditionalColor), value);
        }
        public SolidColorBrush AccentColor
        {
            get => GetSolidColorBrush(nameof(AccentColor));
            set => SetProperty(nameof(AccentColor), value);
        }
        public SolidColorBrush AccentAdditionalColor
        {
            get => GetSolidColorBrush(nameof(AccentAdditionalColor));
            set => SetProperty(nameof(AccentAdditionalColor), value);
        }
        public SolidColorBrush PercentPlusColor
        {
            get => GetSolidColorBrush(nameof(PercentPlusColor));
            set => SetProperty(nameof(PercentPlusColor), value);
        }
        public SolidColorBrush PercentMinusColor
        {
            get => GetSolidColorBrush(nameof(PercentMinusColor));
            set => SetProperty(nameof(PercentMinusColor), value);
        }
        public bool IsGreetingTextVisible
        {
            get => GetProperty<bool>(nameof(IsGreetingTextVisible));
            set => SetProperty(nameof(IsGreetingTextVisible), value);
        }
        public bool IsMenuExpanded
        {
            get => GetProperty<bool>(nameof(IsMenuExpanded));
            set => SetProperty(nameof(IsMenuExpanded), value);
        }
        #endregion Properties

        #region Methods
        public T GetProperty<T>(string propertyName)
        {
            return (T)Convert.ChangeType(_config.AppSettings.Settings[propertyName].Value, typeof(T));
        }
        public T GetEnumProperty<T>(string propertyName) where T : Enum
        {
            return (T)Enum.Parse(typeof(T), _config.AppSettings.Settings[propertyName].Value, true);
        }
        public SolidColorBrush GetSolidColorBrush(string propertyName)
        {
            var converter = new BrushConverter();
            return converter.ConvertFrom(_config.AppSettings.Settings[propertyName].Value) as SolidColorBrush;
        }
        public void SetProperty(string propertyName, object value)
        {
            _config.AppSettings.Settings[propertyName].Value = value.ToString();
            _config.Save();
            ConfigurationManager.RefreshSection("appSettings");
        }
        #endregion Methods
    }
}
