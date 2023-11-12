using CommunityToolkit.Mvvm.ComponentModel;
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
    public class RemainElementModel : ObservableObject
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
        public Remain Remain => _remain;
        public RemainGroup RemainGroup => _remain.IdGroupNavigation;
        public string Url => _remain.IdSkinNavigation.Url;
        public string Title => _remain.IdSkinNavigation.Title;
        public long Count => _remain.Count;
        public double CostPurchase => _remain.CostPurchase;
        public DateTime DatePurchase
        {
            get => _datePurchase;
            set => SetProperty(ref _datePurchase, value);
        }
        public double AmountPurchase
        {
            get => _amountPurchase;
            set => SetProperty(ref _amountPurchase, value);
        }
        public Dictionary<DateTime, double> PriceDynamics
        {
            get => _priceDynamics;
            set => SetProperty(ref _priceDynamics, value);
        }
        public double MinPrice
        {
            get => _minPrice;
            set => SetProperty(ref _minPrice, value);
        }
        public double MaxPrice
        {
            get => _maxPrice;
            set => SetProperty(ref _maxPrice, value);
        }
        public DateTime DateLastUpdate
        {
            get => _dateLastUpdate;
            set => SetProperty(ref _dateLastUpdate, value);
        }
        public double LastCost
        {
            get => _lastCost;
            set => SetProperty(ref _lastCost, value);
        }
        public double CurrentAmount
        {
            get => _currentAmount;
            set => SetProperty(ref _currentAmount, value);
        }
        public double Percent
        {
            get => _percent;
            set => SetProperty(ref _percent, value);
        }
        public List<DataPoint> PriceDynamicsPoints
        {
            get => _priceDynamicsPoints;
            set => SetProperty(ref _priceDynamicsPoints, value);
        }
        #endregion Properties

        #region Constructor
        public RemainElementModel(Remain remain)
        {
            _remain = remain;
            DatePurchase = DateTime.ParseExact(remain.DatePurchase, ProgramConstants.DateTimeFormat, null);
            AmountPurchase = remain.CostPurchase * remain.Count;
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
            PriceDynamics = _priceDynamics.OrderBy(x => x.Key.Ticks).ToDictionary(x => x.Key, x => x.Value);

            MinPrice = PriceDynamics.Min(x => x.Value);
            MaxPrice = PriceDynamics.Max(x => x.Value);
            DateLastUpdate = PriceDynamics.Last().Key;
            LastCost = PriceDynamics.Last().Value;
            CurrentAmount = _lastCost * Count;
            Percent = (LastCost - CostPurchase) / CostPurchase * 100;

            int i = 0;
            _priceDynamicsPoints = new();
            foreach (var item in PriceDynamics)
            {
                _priceDynamicsPoints.Add(new(i, item.Value));
                i++;
            }
            OnPropertyChanged(nameof(PriceDynamicsPoints));
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