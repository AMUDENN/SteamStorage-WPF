using Microsoft.EntityFrameworkCore;
using SteamStorage.Entities;
using SteamStorage.Services.Logger;
using SteamStorage.Utilities;
using System;

namespace SteamStorage.Models.EntityModels
{
    public class ArchiveElementModel
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
        public ArchiveGroup ArchiveGroup => _archive.IdGroupNavigation;
        public string Url => _archive.IdSkinNavigation.Url;
        public string Title => _archive.IdSkinNavigation.Title;
        public DateTime DatePurchase => _datePurchase;
        public DateTime DateSold => _dateSold;
        public long Count => _archive.Count;
        public double CostPurchase => _archive.CostPurchase;
        public double CostSold => _archive.CostSold;
        public double AmountPurchase => _amountPurchase;
        public double AmountSold => _amountSold;
        public double Percent => _percent;
        #endregion Properties

        #region Constructor
        public ArchiveElementModel(Archive archive)
        {
            _archive = archive;
            _datePurchase = DateTime.ParseExact(_archive.DatePurchase, ProgramConstants.DateTimeFormat, null);
            _dateSold = DateTime.ParseExact(_archive.DateSold, ProgramConstants.DateTimeFormat, null);
            _amountPurchase = archive.CostPurchase * archive.Count;
            _amountSold = archive.CostSold * archive.Count;
            _percent = (CostSold - CostPurchase) / CostPurchase * 100;
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
