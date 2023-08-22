using Microsoft.EntityFrameworkCore;
using OxyPlot;
using SteamStorage.Entities;
using SteamStorage.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SteamStorage.Models
{
    public class RemainElementModel
    {
        #region Fields
        private Remain _remain;
        private DateTime _datePurchase;
        private double _amountPurchase;
        private Dictionary<DateTime, double> _priceDynamics;
        private double _minPrice;
        private double _maxPrice;
        private DateTime _dateLastUpdate;
        private double _lastCost;
        private double _currentAmount;
        private double _percent;
        private List<DataPoint> _priceDynamicsPoints;

        private readonly Logger? _logger = Singleton.GetObject<Logger>();
        private readonly Context? _context = Singleton.GetObject<Context>();
        #endregion Fields

        #region Properties
        public RemainGroup RemainGroup => _remain.IdGroupNavigation;
        public string Url => _remain.IdSkinNavigation.Url;
        public string Title => _remain.IdSkinNavigation.Title;
        public Remain Remain => _remain;
        public DateTime DatePurchase => _datePurchase;
        public long Count => _remain.Count;
        public double CostPurchase => _remain.CostPurchase;
        public double AmountPurchase => _amountPurchase;
        public Dictionary<DateTime, double> PriceDynamics => _priceDynamics;
        public double MinPrice => _minPrice;
        public double MaxPrice => _maxPrice;
        public DateTime DateLastUpdate => _dateLastUpdate;
        public double LastCost => _lastCost;
        public double CurrentAmount => _currentAmount;
        public double Percent => _percent;
        public List<DataPoint> PriceDynamicsPoints => _priceDynamicsPoints;
        #endregion Properties

        #region Constructor
        public RemainElementModel(Remain remain)
        {
            this._remain = remain;
            _datePurchase = DateTime.ParseExact(remain.DatePurchase, Constants.DateTimeFormat, null);
            _amountPurchase = remain.CostPurchase * remain.Count;
            UpdatePriceDynamics();
        }
        public RemainElementModel()
        {
            _remain = new();
            _context?.AddRemain(_remain);
        }
        #endregion Constructor

        #region Methods
        private void UpdatePriceDynamics()
        {
            _context?.DBContext.PriceDynamics.LoadAsync();
            _context?.DBContext.Skins.LoadAsync();
            _priceDynamics = _remain.PriceDynamics.ToDictionary(x => DateTime.ParseExact(x.DateUpdate, Constants.DateTimeFormat, null), x => x.CostUpdate);
            _priceDynamics.Add(DatePurchase, CostPurchase);
            if (_priceDynamics.Count == 1) _priceDynamics.Add(DatePurchase.AddMilliseconds(1), CostPurchase);
            _priceDynamics = _priceDynamics.OrderBy(x => x.Key.Ticks).ToDictionary(x => x.Key, x => x.Value);

            _minPrice = _priceDynamics.Values.Min();
            _maxPrice = _priceDynamics.Values.Max();
            _dateLastUpdate = _priceDynamics.Last().Key;
            _lastCost = _priceDynamics.Last().Value;
            _currentAmount = _lastCost * Count;
            _percent = (_lastCost - CostPurchase) / CostPurchase * 100;

            int i = 0;
            _priceDynamicsPoints = new();
            foreach (var item in _priceDynamics)
            {
                _priceDynamicsPoints.Add(new(i, item.Value));
                i++;
            }
        }
        public void EditRemain(string url, long count, double costPurchase, DateTime datePurchase, RemainGroupModel? remainGroupModel)
        {
            try
            {
                var skin = _context?.GetSkin(url);
                if (skin is null) throw new Exception("Ссылка на скин неверна!");
                _remain.IdSkinNavigation = skin;
                _remain.Count = count;
                _remain.CostPurchase = costPurchase;
                _remain.DatePurchase = datePurchase.ToString(Constants.DateTimeFormat);
                _remain.IdGroup = remainGroupModel is null ? 1 : remainGroupModel.RemainGroup.Id;
                _context?.SaveChanges();
                _context?.UpdateRemainModels();
                _logger?.WriteMessage($"Элемент {Title} успешно изменён!", this.GetType());
            }
            catch (Exception ex)
            {
                _context?.UndoChanges();
                _logger?.WriteMessage($"Не удалось изменить элемент {Title}. Ошибка: {ex.Message}", this.GetType());
                UserMessage.Error($"Не удалось изменить элемент {Title}");
            }
        }
        public void EditRemain(RemainGroupModel? remainGroupModel)
        {
            try
            {
                _remain.IdGroup = remainGroupModel is null ? 1 : remainGroupModel.RemainGroup.Id;
                _context?.SaveChanges();
                _context?.UpdateRemainModels();
                _logger?.WriteMessage($"Элемент {Title} успешно изменён!", this.GetType());
            }
            catch (Exception ex)
            {
                _context?.UndoChanges();
                _logger?.WriteMessage($"Не удалось изменить элемент {Title}. Ошибка: {ex.Message}", this.GetType());
                UserMessage.Error($"Не удалось изменить элемент {Title}");
            }
        }
        public void SellRemain(long count, double costSold, DateTime dateSold, ArchiveGroupModel? archiveGroupModel)
        {
            try
            {
                ArchiveElementModel archiveModel = new();
                archiveModel.EditArchive(Url, count, CostPurchase, costSold, DatePurchase, dateSold, archiveGroupModel);
                if (count >= _remain.Count) _context?.RemoveRemain(_remain);
                EditRemain(Url, Count - count, CostPurchase, DatePurchase, _context?.RemainGroupModels.Where(x => x.RemainGroup == RemainGroup).First());
                _logger?.WriteMessage($"Элемент {Title} успешно продан в количестве {count} штук по цене {costSold}!", this.GetType());
            }
            catch (Exception ex)
            {
                _context?.UndoChanges();
                _logger?.WriteMessage($"Не удалось продать элемент {Title}. Ошибка: {ex.Message}", this.GetType());
                UserMessage.Error($"Не удалось продать элемент {Title}");
            }
        }
        public void DeleteRemain()
        {
            try
            {
                _context?.RemoveRemain(_remain);
                _logger?.WriteMessage($"Элемент {Title} успешно удалён!", this.GetType());
            }
            catch (Exception ex)
            {
                _context?.UndoChanges();
                _logger?.WriteMessage($"Не удалось удалить элемент {Title}. Ошибка: {ex.Message}", this.GetType());
                UserMessage.Error($"Не удалось удалить элемент {Title}");
            }
        }
        public void UpdateCurrentCost()
        {
            try
            {
                _context?.AddPriceDynamic(this);
                UpdatePriceDynamics();
                _logger?.WriteMessage($"Текущая цена элемента {Title} успешно добавлена!", this.GetType());
            }
            catch (Exception ex)
            {
                _context?.UndoChanges();
                _logger?.WriteMessage($"Не удалось узнать текущую цену элемента {Title}. Ошибка: {ex.Message}", this.GetType());
                UserMessage.Error($"Не удалось узнать текущую цену элемента {Title}");
            }
        }
        #endregion Methods
    }
}