using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SteamStorage.Utilities;
using System.Linq;

namespace SteamStorage.ViewModels
{
    public class SettingsVM : ObservableObject
    {
        #region Fields
        private bool isDarkTheme;
        private bool isLightTheme;
        private bool isCustomTheme;
        private string mainColor;
        private string mainAdditionalColor;
        private string additionalColor;
        private string accentColor;
        private string accentAdditionalColor;
        private string percentPlusColor;
        private string percentMinusColor;
        private RelayCommand saveColorsCommand;
        private RelayCommand resetColorsCommand;
        private RelayCommand openLogCommand;
        private RelayCommand clearDatabaseCommand;
        #endregion Fields

        #region Properties
        public bool IsDarkTheme
        {
            get => isDarkTheme;
            set
            {
                SetProperty(ref isDarkTheme, value);
                ChangeTheme();
            }
        }
        public bool IsLightTheme
        {
            get => isLightTheme;
            set
            {
                SetProperty(ref isLightTheme, value);
                ChangeTheme();
            }
        }
        public bool IsCustomTheme
        {
            get => isCustomTheme;
            set
            {
                SetProperty(ref isCustomTheme, value);
                ChangeTheme();
            }
        }
        public string MainColor
        {
            get => mainColor;
            set
            {
                SetProperty(ref mainColor, value.ToUpper());
                SaveColorsCommand.NotifyCanExecuteChanged();
            }
        }
        public string MainAdditionalColor
        {
            get => mainAdditionalColor;
            set
            {
                SetProperty(ref mainAdditionalColor, value.ToUpper()); 
                SaveColorsCommand.NotifyCanExecuteChanged();
            }
        }
        public string AdditionalColor
        {
            get => additionalColor;
            set
            {
                SetProperty(ref additionalColor, value.ToUpper());
                SaveColorsCommand.NotifyCanExecuteChanged();
            }
        }
        public string AccentColor
        {
            get => accentColor;
            set
            {
                SetProperty(ref accentColor, value.ToUpper());
                SaveColorsCommand.NotifyCanExecuteChanged();
            }
        }
        public string AccentAdditionalColor
        {
            get => accentAdditionalColor;
            set 
            {
                SetProperty(ref accentAdditionalColor, value.ToUpper());
                SaveColorsCommand.NotifyCanExecuteChanged();
            }
        }
        public string PercentPlusColor
        {
            get => percentPlusColor;
            set 
            {
                SetProperty(ref percentPlusColor, value.ToUpper());
                SaveColorsCommand.NotifyCanExecuteChanged();
            }
        }
        public string PercentMinusColor
        {
            get => percentMinusColor;
            set 
            {
                SetProperty(ref percentMinusColor, value.ToUpper());
                SaveColorsCommand.NotifyCanExecuteChanged();
            }
        }
        #endregion Properties

        #region Commands
        public RelayCommand SaveColorsCommand
        {
            get
            {
                return saveColorsCommand ??= new RelayCommand(DoSaveColorsCommand, CanExecuteSaveColorsCommand);
            }
        }
        public RelayCommand ResetColorsCommand
        {
            get
            {
                return resetColorsCommand ??= new RelayCommand(DoResetColorsCommand);
            }
        }
        public RelayCommand OpenLogCommand
        {
            get
            {
                return openLogCommand ??= new RelayCommand(DoOpenLogCommand);
            }
        }
        public RelayCommand ClearDatabaseCommand
        {
            get
            {
                return clearDatabaseCommand ??= new RelayCommand(DoClearDatabaseCommand);
            }
        }
        #endregion Commands

        #region Constructor
        public SettingsVM()
        {
            MainColor = Config.MainColor;
            MainAdditionalColor = Config.MainAdditionalColor;
            AdditionalColor = Config.AdditionalColor;
            AccentColor = Config.AccentColor;
            AccentAdditionalColor = Config.AccentAdditionalColor;
            PercentPlusColor = Config.PercentPlusColor;
            PercentMinusColor = Config.PercentMinusColor;

            var currentTheme = Config.CurrentTheme;
            IsLightTheme = currentTheme == "Light";
            IsDarkTheme = currentTheme == "Dark";
            IsCustomTheme = currentTheme == "Custom";
        }
        #endregion Constructor

        #region Methods
        private void DoSaveColorsCommand()
        {
            Config.MainColor = MainColor;
            Config.MainAdditionalColor = MainAdditionalColor;
            Config.AdditionalColor = AdditionalColor;
            Config.AccentColor = AccentColor;
            Config.AccentAdditionalColor = AccentAdditionalColor;
            Config.PercentPlusColor = PercentPlusColor;
            Config.PercentMinusColor = PercentMinusColor;
            Themes.SetCustomColors();
        }
        private bool CanExecuteSaveColorsCommand()
        {
            var colors = new string[] { MainColor, MainAdditionalColor, AdditionalColor, AccentColor,
                AccentAdditionalColor, PercentPlusColor, PercentMinusColor };
            if (colors.Where(x => x.Length != 6).Any()
                || colors.Distinct().Count() != colors.Length)
                return false;
            return true;
        }
        private void DoResetColorsCommand()
        {
            MainColor = "7371FF";
            MainAdditionalColor = "00AD71";
            AdditionalColor = "FFFFFF";
            AccentColor = "0D1117";
            AccentAdditionalColor = "21262D";
            PercentPlusColor = "02B478";
            PercentMinusColor = "FD4534";
        }
        private void DoOpenLogCommand()
        {

        }
        private void DoClearDatabaseCommand()
        {

        }
        private void ChangeTheme()
        {
            if (IsLightTheme) Themes.ChangeTheme(Themes.ThemesEnum.Light);
            if (IsDarkTheme) Themes.ChangeTheme(Themes.ThemesEnum.Dark);
            if (IsCustomTheme) Themes.ChangeTheme(Themes.ThemesEnum.Custom);
        }
        #endregion Methods
    }
}
