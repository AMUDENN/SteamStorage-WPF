using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SteamStorage.Utilities;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace SteamStorage.ViewModels
{
    public class SettingsVM : ObservableObject
    {
        #region Fields
        private bool _isDarkTheme;
        private bool _isLightTheme;
        private bool _isCustomTheme;

        private string _mainColor;
        private string _mainAdditionalColor;
        private string _additionalColor;
        private string _accentColor;
        private string _accentAdditionalColor;
        private string _percentPlusColor;
        private string _percentMinusColor;

        private RelayCommand _exportToDB;
        private RelayCommand _exportToExcel;
        private RelayCommand _saveColorsCommand;
        private RelayCommand _resetColorsCommand;
        private RelayCommand _openLogCommand;
        private RelayCommand _clearDatabaseCommand;
        #endregion Fields

        #region Properties
        public bool IsDarkTheme
        {
            get => _isDarkTheme;
            set
            {
                SetProperty(ref _isDarkTheme, value);
                ChangeTheme();
            }
        }
        public bool IsLightTheme
        {
            get => _isLightTheme;
            set
            {
                SetProperty(ref _isLightTheme, value);
                ChangeTheme();
            }
        }
        public bool IsCustomTheme
        {
            get => _isCustomTheme;
            set
            {
                SetProperty(ref _isCustomTheme, value);
                ChangeTheme();
            }
        }
        public string MainColor
        {
            get => _mainColor;
            set
            {
                SetProperty(ref _mainColor, value.ToUpper());
                SaveColorsCommand.NotifyCanExecuteChanged();
            }
        }
        public string MainAdditionalColor
        {
            get => _mainAdditionalColor;
            set
            {
                SetProperty(ref _mainAdditionalColor, value.ToUpper()); 
                SaveColorsCommand.NotifyCanExecuteChanged();
            }
        }
        public string AdditionalColor
        {
            get => _additionalColor;
            set
            {
                SetProperty(ref _additionalColor, value.ToUpper());
                SaveColorsCommand.NotifyCanExecuteChanged();
            }
        }
        public string AccentColor
        {
            get => _accentColor;
            set
            {
                SetProperty(ref _accentColor, value.ToUpper());
                SaveColorsCommand.NotifyCanExecuteChanged();
            }
        }
        public string AccentAdditionalColor
        {
            get => _accentAdditionalColor;
            set 
            {
                SetProperty(ref _accentAdditionalColor, value.ToUpper());
                SaveColorsCommand.NotifyCanExecuteChanged();
            }
        }
        public string PercentPlusColor
        {
            get => _percentPlusColor;
            set 
            {
                SetProperty(ref _percentPlusColor, value.ToUpper());
                SaveColorsCommand.NotifyCanExecuteChanged();
            }
        }
        public string PercentMinusColor
        {
            get => _percentMinusColor;
            set 
            {
                SetProperty(ref _percentMinusColor, value.ToUpper());
                SaveColorsCommand.NotifyCanExecuteChanged();
            }
        }
        #endregion Properties

        #region Commands
        public RelayCommand ExportToDB
        {
            get
            {
                return _exportToDB ??= new RelayCommand(DoExportToDBCommand);
            }
        }
        public RelayCommand ExportToExcel
        {
            get
            {
                return _exportToExcel ??= new RelayCommand(DoExportToExcelCommand);
            }
        }
        public RelayCommand SaveColorsCommand
        {
            get
            {
                return _saveColorsCommand ??= new RelayCommand(DoSaveColorsCommand, CanExecuteSaveColorsCommand);
            }
        }
        public RelayCommand ResetColorsCommand
        {
            get
            {
                return _resetColorsCommand ??= new RelayCommand(DoResetColorsCommand);
            }
        }
        public RelayCommand OpenLogCommand
        {
            get
            {
                return _openLogCommand ??= new RelayCommand(DoOpenLogCommand);
            }
        }
        public RelayCommand ClearDatabaseCommand
        {
            get
            {
                return _clearDatabaseCommand ??= new RelayCommand(DoClearDatabaseCommand);
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
        private void DoExportToDBCommand()
        {

        }
        private void DoExportToExcelCommand()
        {

        }
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
            if (colors.Where(x => x.Length % 3 != 0).Any()
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
            if (File.Exists(Constants.Logpath))
                Process.Start(@"notepad.exe", Constants.Logpath);
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
