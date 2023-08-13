using Microsoft.EntityFrameworkCore;
using SteamStorage.Entities;
using SteamStorage.Utilities;
using System;

namespace SteamStorage.Models
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

        private readonly Logger? _logger = Singleton.GetObject<Logger>();
        private readonly Context? _context = Singleton.GetObject<Context>();
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
            this._archive = archive;
            _datePurchase = DateTime.ParseExact(this._archive.DatePurchase, Constants.DateTimeFormat, null);
            _dateSold = DateTime.ParseExact(this._archive.DateSold, Constants.DateTimeFormat, null);
            _amountPurchase = archive.CostPurchase * archive.Count;
            _amountSold = archive.CostSold * archive.Count;
            _percent = (CostSold - CostPurchase) / CostPurchase * 100;
            _context?.DBContext.Skins.LoadAsync();
        }
        public ArchiveElementModel()
        {
            _archive = new();
            _context?.AddArchive(_archive);
        }
        #endregion Constructor

        #region Methods
        public void EditArchive(string url, long count, double costPurchase, double costSold, DateTime datePurchase, DateTime dateSold, ArchiveGroupModel? archiveGroupModel)
        {
            try
            {
                var skin = _context?.GetSkin(url);
                if (skin is null) throw new Exception("Ссылка на скин неверна!");
                _archive.IdSkinNavigation = skin;
                _archive.Count = count;
                _archive.CostPurchase = costPurchase;
                _archive.CostSold = costSold;
                _archive.DatePurchase = datePurchase.ToString(Constants.DateTimeFormat);
                _archive.DateSold = dateSold.ToString(Constants.DateTimeFormat);
                _archive.IdGroup = archiveGroupModel is null ? 1 : archiveGroupModel.ArchiveGroup.Id;
                _context?.SaveChanges();
                _logger?.WriteMessage($"Элемент {Title} успешно изменён!", this.GetType());
            }
            catch (Exception ex)
            {
                _context?.UndoChanges();
                _logger?.WriteMessage($"Не удалось изменить элемент {Title}. Ошибка: {ex.Message}", this.GetType());
            }
        }
        public void DeleteArchive()
        {
            try
            {
                _context?.RemoveArchive(_archive);
                _context?.SaveChanges();
                _logger?.WriteMessage($"Элемент {Title} успешно удалён!", this.GetType());
            }
            catch (Exception ex)
            {
                _context?.UndoChanges();
                _logger?.WriteMessage($"Не удалось удалить элемент {Title}. Ошибка: {ex.Message}", this.GetType());
            }
        }
        #endregion Methods
    }
}
