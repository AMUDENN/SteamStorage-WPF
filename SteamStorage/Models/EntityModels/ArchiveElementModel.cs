using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;
using SteamStorage.Entities;
using SteamStorage.Services.Logger;
using SteamStorage.Utilities;
using System;

namespace SteamStorage.Models.EntityModels
{
    public class ArchiveElementModel : ObservableObject
    {
        #region Fields
        private Archive _archive;
        private DateTime _datePurchase;
        private DateTime _dateSold;
        private double _amountPurchase;
        private double _amountSold;
        private double _percent;

        private readonly LoggerService? _loggerService = Singleton.GetService<LoggerService>();
        private readonly Context? _context = Singleton.GetService<Context>();
        #endregion Fields

        #region Properties
        public Archive Archive => _archive;
        public ArchiveGroup ArchiveGroup => _archive.IdGroupNavigation;
        public string Url => _archive.IdSkinNavigation.Url;
        public string Title => _archive.IdSkinNavigation.Title;
        public long Count => _archive.Count;
        public double CostPurchase => _archive.CostPurchase;
        public double CostSold => _archive.CostSold;
        public DateTime DatePurchase
        {
            get => _datePurchase;
            set => SetProperty(ref _datePurchase, value);
        }
        public DateTime DateSold
        {
            get => _dateSold;
            set => SetProperty(ref _dateSold, value);
        }
        public double AmountPurchase
        {
            get => _amountPurchase;
            set => SetProperty(ref _amountPurchase, value);
        }
        public double AmountSold 
        {
            get => _amountSold; 
            set => SetProperty(ref _amountSold, value);
        }
        public double Percent 
        {
            get => _percent; 
            set => SetProperty(ref _percent, value);
        }
        #endregion Properties

        #region Constructor
        public ArchiveElementModel(Archive archive)
        {
            _archive = archive;
            DatePurchase = DateTime.ParseExact(_archive.DatePurchase, ProgramConstants.DateTimeFormat, null);
            DateSold = DateTime.ParseExact(_archive.DateSold, ProgramConstants.DateTimeFormat, null);
            AmountPurchase = archive.CostPurchase * archive.Count;
            AmountSold = archive.CostSold * archive.Count;
            Percent = (CostSold - CostPurchase) / CostPurchase * 100;
            _context?.DBContext.Skins.LoadAsync();
        }
        public ArchiveElementModel(string url, long count, double costPurchase, double costSold, DateTime datePurchase, DateTime dateSold, ArchiveGroupElementModel? archiveGroupModel)
        {
            _archive = new();
            EditArchive(url, count, costPurchase, costSold, datePurchase, dateSold, archiveGroupModel);
            _context?.AddArchive(_archive);
        }
        #endregion Constructor

        #region Methods
        public void EditArchive(string url, long count, double costPurchase, double costSold, DateTime datePurchase, DateTime dateSold, ArchiveGroupElementModel? archiveGroupModel)
        {
            try
            {
                _context?.EditArchive(_archive, url, count, costPurchase, costSold, datePurchase, dateSold, archiveGroupModel);
                _loggerService?.WriteMessage($"Элемент {Title} успешно изменён!", GetType());
            }
            catch (Exception ex)
            {
                _context?.UndoChanges();
                _loggerService?.WriteMessage($"Не удалось изменить элемент {Title}. Ошибка: {ex.Message}", GetType());
                UserMessage.Error($"Не удалось изменить элемент {Title}");
            }
        }
        public void EditArchive(ArchiveGroupElementModel? archiveGroupModel)
        {
            try
            {
                _context?.EditArchive(_archive, archiveGroupModel);
                _loggerService?.WriteMessage($"Элемент {Title} успешно изменён!", GetType());
            }
            catch (Exception ex)
            {
                _context?.UndoChanges();
                _loggerService?.WriteMessage($"Не удалось изменить элемент {Title}. Ошибка: {ex.Message}", GetType());
                UserMessage.Error($"Не удалось изменить элемент {Title}");
            }
        }
        public void DeleteArchive()
        {
            try
            {
                _context?.RemoveArchive(_archive);
                _loggerService?.WriteMessage($"Элемент {Title} успешно удалён!", GetType());
            }
            catch (Exception ex)
            {
                _context?.UndoChanges();
                _loggerService?.WriteMessage($"Не удалось удалить элемент {Title}. Ошибка: {ex.Message}", GetType());
                UserMessage.Error($"Не удалось удалить элемент {Title}");
            }
        }
        #endregion Methods
    }
}
