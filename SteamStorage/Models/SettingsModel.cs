using CommunityToolkit.Mvvm.ComponentModel;
using OfficeOpenXml;
using SteamStorage.Models.EntityModels;
using SteamStorage.Services.Config;
using SteamStorage.Services.Dialog;
using SteamStorage.Services.Logger;
using SteamStorage.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using static SteamStorage.Utilities.Themes;

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

        private readonly Context? _context = Singleton.GetService<Context>();
        private readonly ConfigService? _configService = Singleton.GetService<ConfigService>();
        private readonly WindowDialogService? _windowDialogService = Singleton.GetService<WindowDialogService>();
        private readonly LoggerService? _loggerService = Singleton.GetService<LoggerService>();
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
            MainColor = _configService.MainColor;
            MainAdditionalColor = _configService.MainAdditionalColor;
            AdditionalColor = _configService.AdditionalColor;
            AccentColor = _configService.AccentColor;
            AccentAdditionalColor = _configService.AccentAdditionalColor;
            PercentPlusColor = _configService.PercentPlusColor;
            PercentMinusColor = _configService.PercentMinusColor;

            var currentTheme = _configService.CurrentTheme;
            IsLightTheme = currentTheme == ThemesEnum.Light;
            IsDarkTheme = currentTheme == ThemesEnum.Dark;
            IsCustomTheme = currentTheme == ThemesEnum.Custom;
        }
        #endregion Constructor

        #region Methods
        public void ExportToExcel()
        {
            try
            {
                if (!_windowDialogService?.SaveFileDialog("Excel file (.xlsx)|*.xlsx") ?? true) return;

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var package = new ExcelPackage())
                {
                    var remainWorksheet = package.Workbook.Worksheets.Add("Остатки");

                    remainWorksheet.Cells["A1"].Value = "Название";
                    remainWorksheet.Cells["B1"].Value = "Количество";
                    remainWorksheet.Cells["C1"].Value = "Цена покупки";
                    remainWorksheet.Cells["D1"].Value = "Сумма";
                    remainWorksheet.Cells["E1"].Value = "Дата покупки";
                    remainWorksheet.Cells["F1"].Value = "Текущая цена";
                    remainWorksheet.Cells["G1"].Value = "Дата обновления";
                    remainWorksheet.Cells["H1"].Value = "Изменение (%)";
                    remainWorksheet.Cells["I1"].Value = "Ссылка";
                    remainWorksheet.Cells["A1:I1"].Style.Font.Bold = true;
                    remainWorksheet.Cells["A1:I1"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

                    List<RemainElementModel> remains = _context.RemainElementModels.ToList();

                    int i = 2;
                    foreach (var item in remains)
                    {
                        remainWorksheet.Cells[i, 1].Value = item.Title;
                        remainWorksheet.Cells[i, 2].Value = item.Count;
                        remainWorksheet.Cells[i, 3].Value = item.CostPurchase;
                        remainWorksheet.Cells[i, 4].Value = item.AmountPurchase;
                        remainWorksheet.Cells[i, 5].Value = item.DatePurchase.ToString(ProgramConstants.DateFormat);
                        remainWorksheet.Cells[i, 6].Value = item.LastCost;
                        remainWorksheet.Cells[i, 7].Value = item.DateLastUpdate.ToString(ProgramConstants.DateFormat);
                        remainWorksheet.Cells[i, 8].Value = (item.Percent > 0 ? "+" : "") + Math.Round(item.Percent, 2);
                        remainWorksheet.Cells[i, 9].Value = item.Url;
                        i++;
                    }

                    remainWorksheet.Cells[i, 2].Value = "Общее количество";
                    remainWorksheet.Cells[i, 3].Value = "Средняя цена покупки";
                    remainWorksheet.Cells[i, 4].Value = "Общая сумма покупки";
                    remainWorksheet.Cells[i, 6].Value = "Средняя текущая цена";
                    remainWorksheet.Cells[i, 7].Value = "Общая текущая стоимость";
                    remainWorksheet.Cells[i, 8].Value = "Общее изменение";

                    remainWorksheet.Cells[i + 1, 1].Value = "Итого";
                    remainWorksheet.Cells[i + 1, 2].Value = CalculationModel.GetRemainTotalCount(remains);
                    remainWorksheet.Cells[i + 1, 3].Value = Math.Round(CalculationModel.GetRemainAverageCostPurchase(remains), 2);
                    remainWorksheet.Cells[i + 1, 4].Value = CalculationModel.GetRemainTotalAmountPurchase(remains);
                    remainWorksheet.Cells[i + 1, 6].Value = Math.Round(CalculationModel.GetRemainAverageCurrentCost(remains), 2);
                    remainWorksheet.Cells[i + 1, 7].Value = CalculationModel.GetRemainTotalCurrentAmount(remains);
                    remainWorksheet.Cells[i + 1, 8].Value = Math.Round(CalculationModel.GetRemainAveragePercent(remains), 2);

                    remainWorksheet.Cells[i, 1, i + 1, 8].Style.Font.Bold = true;
                    remainWorksheet.Cells[i, 1, i + 1, 8].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

                    remainWorksheet.Cells[1, 1, i + 1, 9].AutoFitColumns();



                    var archiveWorksheet = package.Workbook.Worksheets.Add("Архив");

                    archiveWorksheet.Cells["A1"].Value = "Название";
                    archiveWorksheet.Cells["B1"].Value = "Количество";
                    archiveWorksheet.Cells["C1"].Value = "Цена покупки";
                    archiveWorksheet.Cells["D1"].Value = "Сумма покупки";
                    archiveWorksheet.Cells["E1"].Value = "Дата покупки";
                    archiveWorksheet.Cells["F1"].Value = "Цена продажи";
                    archiveWorksheet.Cells["G1"].Value = "Сумма продажи";
                    archiveWorksheet.Cells["H1"].Value = "Дата продажи";
                    archiveWorksheet.Cells["I1"].Value = "Изменение (%)";
                    archiveWorksheet.Cells["J1"].Value = "Ссылка";
                    archiveWorksheet.Cells["A1:J1"].Style.Font.Bold = true;
                    archiveWorksheet.Cells["A1:J1"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

                    List<ArchiveElementModel> archives = _context.ArchiveElementModels.ToList();

                    int j = 2;
                    foreach (var item in archives)
                    {
                        archiveWorksheet.Cells[j, 1].Value = item.Title;
                        archiveWorksheet.Cells[j, 2].Value = item.Count;
                        archiveWorksheet.Cells[j, 3].Value = item.CostPurchase;
                        archiveWorksheet.Cells[j, 4].Value = item.AmountPurchase;
                        archiveWorksheet.Cells[j, 5].Value = item.DatePurchase.ToString(ProgramConstants.DateFormat);
                        archiveWorksheet.Cells[j, 6].Value = item.CostSold;
                        archiveWorksheet.Cells[j, 7].Value = item.AmountSold;
                        archiveWorksheet.Cells[j, 8].Value = item.DateSold.ToString(ProgramConstants.DateFormat);
                        archiveWorksheet.Cells[j, 9].Value = (item.Percent > 0 ? "+" : "") + Math.Round(item.Percent, 2);
                        archiveWorksheet.Cells[j, 10].Value = item.Url;
                        j++;
                    }

                    archiveWorksheet.Cells[j, 2].Value = "Общее количество";
                    archiveWorksheet.Cells[j, 3].Value = "Средняя цена покупки";
                    archiveWorksheet.Cells[j, 4].Value = "Общая сумма покупки";
                    archiveWorksheet.Cells[j, 6].Value = "Средняя цена продажи";
                    archiveWorksheet.Cells[j, 7].Value = "Общая сумма продажи";
                    archiveWorksheet.Cells[j, 9].Value = "Общее изменение";

                    archiveWorksheet.Cells[j + 1, 1].Value = "Итого";
                    archiveWorksheet.Cells[j + 1, 2].Value = CalculationModel.GetArchiveTotalCount(archives);
                    archiveWorksheet.Cells[j + 1, 3].Value = Math.Round(CalculationModel.GetArchiveAverageCostPurchase(archives), 2);
                    archiveWorksheet.Cells[j + 1, 4].Value = CalculationModel.GetArchiveTotalAmountPurchase(archives);
                    archiveWorksheet.Cells[j + 1, 6].Value = Math.Round(CalculationModel.GetArchiveAverageCostSold(archives), 2);
                    archiveWorksheet.Cells[j + 1, 7].Value = CalculationModel.GetArchiveTotalAmountSold(archives);
                    archiveWorksheet.Cells[j + 1, 9].Value = Math.Round(CalculationModel.GetArchiveAveragePercent(archives), 2);

                    archiveWorksheet.Cells[j, 1, j + 1, 9].Style.Font.Bold = true;
                    archiveWorksheet.Cells[j, 1, j + 1, 9].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

                    archiveWorksheet.Cells[1, 1, j + 1, 10].AutoFitColumns();

                    byte[] data = package.GetAsByteArray();

                    using FileStream fs = new(_windowDialogService.FilePath, FileMode.Create);
                    package.SaveAs(fs);
                }
                _loggerService?.WriteMessage("Экспорт в эксель завершился успешно!", GetType());
                UserMessage.Information("Экспорт в эксель завершился успешно!");
            }
            catch (Exception ex)
            {
                _loggerService?.WriteMessage(ex, $"Экспорт в эксель завершился с ошибкой!");
                UserMessage.Error("Экспорт в эксель завершился с ошибкой!");
            }
        }
        public void SaveColors()
        {
            _configService.MainColor = MainColor;
            _configService.MainAdditionalColor = MainAdditionalColor;
            _configService.AdditionalColor = AdditionalColor;
            _configService.AccentColor = AccentColor;
            _configService.AccentAdditionalColor = AccentAdditionalColor;
            _configService.PercentPlusColor = PercentPlusColor;
            _configService.PercentMinusColor = PercentMinusColor;
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
            if (File.Exists(ProgramConstants.LogPath))
                Process.Start(@"notepad.exe", ProgramConstants.LogPath);
        }
        public void ClearDatabase()
        {
            var delete = UserMessage.TextConfirmation("Вы уверены, что хотите очистить базу данных? \nВсе данные будут удалены без возможности восстановления!", "УДАЛИТЬ");
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
