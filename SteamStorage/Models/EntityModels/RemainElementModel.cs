using Microsoft.EntityFrameworkCore;
using OxyPlot;
using SteamStorage.Entities;
using SteamStorage.Services.Logger;
using SteamStorage.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SteamStorage.Models.EntityModels
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

        private readonly LoggerService? _loggerService = Singleton.GetService<LoggerService>();
        private readonly Context? _context = Singleton.GetService<Context>();
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
            _remain = remain;
            _datePurchase = DateTime.ParseExact(remain.DatePurchase, ProgramConstants.DateTimeFormat, null);
            _amountPurchase = remain.CostPurchase * remain.Count;
            UpdatePriceDynamics();
        }
        public RemainElementModel(string url, long count, double costPurchase, DateTime datePurchase, RemainGroupElementModel? remainGroupModel)
        {
            _remain = new();
            EditRemain(url, count, costPurchase, datePurchase, remainGroupModel);
            _context?.AddRemain(_remain);
        }
        #endregion Constructor

        #region Methods
        private void UpdatePriceDynamics()
        {
            _context?.DBContext.PriceDynamics.LoadAsync();
            _context?.DBContext.Skins.LoadAsync();
            _priceDynamics = _remain.PriceDynamics.ToDictionary(x => DateTime.ParseExact(x.DateUpdate, ProgramConstants.DateTimeFormat, null), x => x.CostUpdate);
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
        public void EditRemain(string url, long count, double costPurchase, DateTime datePurchase, RemainGroupElementModel? remainGroupModel)
        {
            try
            {
                _context?.EditRemain(_remain, url, count, costPurchase, datePurchase, remainGroupModel);
                _loggerService?.WriteMessage($"Элемент {Title} успешно изменён!", GetType());
            }
            catch (Exception ex)
            {
                _context?.UndoChanges();
                _loggerService?.WriteMessage($"Не удалось изменить элемент {Title}. Ошибка: {ex.Message}", GetType());
                UserMessage.Error($"Не удалось изменить элемент {Title}");
            }
        }
        public void EditRemain(RemainGroupElementModel? remainGroupModel)
        {
            try
            {
                _context?.EditRemain(_remain, remainGroupModel);
                _loggerService?.WriteMessage($"Элемент {Title} успешно изменён!", GetType());
            }
            catch (Exception ex)
            {
                _context?.UndoChanges();
                _loggerService?.WriteMessage($"Не удалось изменить элемент {Title}. Ошибка: {ex.Message}", GetType());
                UserMessage.Error($"Не удалось изменить элемент {Title}");
            }
        }
        public void SellRemain(long count, double costSold, DateTime dateSold, ArchiveGroupElementModel? archiveGroupModel)
        {
            try
            {
                ArchiveElementModel archiveModel = new(Url, count, CostPurchase, costSold, DatePurchase, dateSold, archiveGroupModel);
                if (count >= _remain.Count) _context?.RemoveRemain(_remain);
                EditRemain(Url, Count - count, CostPurchase, DatePurchase, _context?.RemainGroupModels.Where(x => x.RemainGroup == RemainGroup).First());
                _loggerService?.WriteMessage($"Элемент {Title} успешно продан в количестве {count} штук по цене {costSold}!", GetType());
            }
            catch (Exception ex)
            {
                _context?.UndoChanges();
                _loggerService?.WriteMessage($"Не удалось продать элемент {Title}. Ошибка: {ex.Message}", GetType());
                UserMessage.Error($"Не удалось продать элемент {Title}");
            }
        }
        public void DeleteRemain()
        {
            try
            {
                _context?.RemoveRemain(_remain);
                _loggerService?.WriteMessage($"Элемент {Title} успешно удалён!", GetType());
            }
            catch (Exception ex)
            {
                _context?.UndoChanges();
                _loggerService?.WriteMessage($"Не удалось удалить элемент {Title}. Ошибка: {ex.Message}", GetType());
                UserMessage.Error($"Не удалось удалить элемент {Title}");
            }
        }
        public void UpdateCurrentCost()
        {
            try
            {
                _context?.AddPriceDynamic(this);
                UpdatePriceDynamics();
                _loggerService?.WriteMessage($"Текущая цена элемента {Title} успешно добавлена!", GetType());
            }
            catch (Exception ex)
            {
                _context?.UndoChanges();
                _loggerService?.WriteMessage($"Не удалось узнать текущую цену элемента {Title}. Ошибка: {ex.Message}", GetType());
                UserMessage.Error($"Не удалось узнать текущую цену элемента {Title}");
            }
        }
        #endregion Methods
    }
}