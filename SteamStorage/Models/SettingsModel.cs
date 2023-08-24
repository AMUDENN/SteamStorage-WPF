﻿using CommunityToolkit.Mvvm.ComponentModel;
using SteamStorage.Utilities;
using System.Diagnostics;
using System.IO;
using OfficeOpenXml;
using System.Drawing;
using OfficeOpenXml.Style;
using System;
using System.IO.Packaging;
using System.Linq.Expressions;

namespace SteamStorage.Models
{
    public class SettingsModel : ObservableObject
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

        private readonly Context? _context = Singleton.GetObject<Context>();
        private readonly Logger? _logger = Singleton.GetObject<Logger>();
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
            }
        }
        public string MainAdditionalColor
        {
            get => _mainAdditionalColor;
            set
            {
                SetProperty(ref _mainAdditionalColor, value.ToUpper());
            }
        }
        public string AdditionalColor
        {
            get => _additionalColor;
            set
            {
                SetProperty(ref _additionalColor, value.ToUpper());
            }
        }
        public string AccentColor
        {
            get => _accentColor;
            set
            {
                SetProperty(ref _accentColor, value.ToUpper());
            }
        }
        public string AccentAdditionalColor
        {
            get => _accentAdditionalColor;
            set
            {
                SetProperty(ref _accentAdditionalColor, value.ToUpper());
            }
        }
        public string PercentPlusColor
        {
            get => _percentPlusColor;
            set
            {
                SetProperty(ref _percentPlusColor, value.ToUpper());
            }
        }
        public string PercentMinusColor
        {
            get => _percentMinusColor;
            set
            {
                SetProperty(ref _percentMinusColor, value.ToUpper());
            }
        }
        #endregion Properties

        #region Constructor
        public SettingsModel()
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
        public void ExportToExcel()
        {
            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var package = new ExcelPackage())
                {
                    var remainWorksheet = package.Workbook.Worksheets.Add("Остатки");

                    var archiveWorksheet = package.Workbook.Worksheets.Add("Архив");

                    byte[] data = package.GetAsByteArray();

                    //Возможно стоит сделать SaveFileDialog -_-
                    using FileStream fs = new($"{Constants.Exportpath}skins ({DateTime.Now.ToString(Constants.DateTimeFormatForExport)}).xlsx", FileMode.Create);
                    package.SaveAs(fs);

                }
                _logger?.WriteMessage("Экспорт в эксель завершился успешно! Найти файл можно в папке Export.", this.GetType());
                UserMessage.Information("Экспорт в эксель завершился успешно! Найти файл можно в папке Export.");
            }
            catch (Exception ex)
            {
                _logger?.WriteMessage($"Экспорт в эксель завершился с ошибкой! {ex.Message}", this.GetType());
                UserMessage.Error("Экспорт в эксель завершился с ошибкой!");
            }
        }
        public void SaveColors()
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
        public void ResetColors()
        {
            MainColor = "7371FF";
            MainAdditionalColor = "00AD71";
            AdditionalColor = "FFFFFF";
            AccentColor = "0D1117";
            AccentAdditionalColor = "21262D";
            PercentPlusColor = "02B478";
            PercentMinusColor = "FD4534";
        }
        public void OpenLog()
        {
            if (File.Exists(Constants.Logpath))
                Process.Start(@"notepad.exe", Constants.Logpath);
        }
        public void ClearDatabase()
        {
            var delete = UserMessage.Question("Вы уверены, что хотите очистить базу данных? \nВсе данные будут удалены без возможности восстановления!", 250, 450);
            if (!delete) return;
            _context?.ClearDatabase();
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
