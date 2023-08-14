using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SteamStorage.Models;
using System.Linq;

namespace SteamStorage.ViewModels
{
    public class SettingsVM : ObservableObject
    {
        #region Fields
        private SettingsModel _settingsModel = new();

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
            get => _settingsModel.IsDarkTheme;
            set => _settingsModel.IsDarkTheme = value;
        }
        public bool IsLightTheme
        {
            get => _settingsModel.IsLightTheme;
            set => _settingsModel.IsLightTheme = value;
        }
        public bool IsCustomTheme
        {
            get => _settingsModel.IsCustomTheme;
            set => _settingsModel.IsCustomTheme = value;
        }
        public string MainColor
        {
            get => _settingsModel.MainColor;
            set => _settingsModel.MainColor = value;
        }
        public string MainAdditionalColor
        {
            get => _settingsModel.MainAdditionalColor;
            set => _settingsModel.MainAdditionalColor = value;
        }
        public string AdditionalColor
        {
            get => _settingsModel.AdditionalColor;
            set => _settingsModel.AdditionalColor = value;
        }
        public string AccentColor
        {
            get => _settingsModel.AccentColor;
            set => _settingsModel.AccentColor = value;
        }
        public string AccentAdditionalColor
        {
            get => _settingsModel.AccentAdditionalColor;
            set => _settingsModel.AccentAdditionalColor = value;
        }
        public string PercentPlusColor
        {
            get => _settingsModel.PercentPlusColor;
            set => _settingsModel.PercentPlusColor = value;
        }
        public string PercentMinusColor
        {
            get => _settingsModel.PercentMinusColor;
            set => _settingsModel.PercentMinusColor = value;
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
            _settingsModel.PropertyChanged += (s, e) =>
            {
                OnPropertyChanged(e.PropertyName);
                SaveColorsCommand.NotifyCanExecuteChanged();
            };
        }
        #endregion Constructor

        #region Methods
        private void DoExportToDBCommand()
        {
            _settingsModel.ExportToDB();
        }
        private void DoExportToExcelCommand()
        {
            _settingsModel.ExportToExcel();
        }
        private void DoSaveColorsCommand()
        {
            _settingsModel.SaveColors();
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
            _settingsModel.ResetColors();
        }
        private void DoOpenLogCommand()
        {
            _settingsModel.OpenLog();
        }
        private void DoClearDatabaseCommand()
        {
            _settingsModel.ClearDatabase();
        }
        #endregion Methods
    }
}
