using Microsoft.EntityFrameworkCore;
using OxyPlot;
using SteamStorage.Entities;
using SteamStorage.Utilities;
using SteamStorage.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SteamStorage.Models
{
    public class RemainModel
    {
        #region Fields
        private Remain remain;
        private DateTime datePurchase;
        private double amountPurchase;
        private Dictionary<DateTime, double> priceDynamics;
        private double minPrice;
        private double maxPrice;
        private DateTime dateLastUpdate;
        private double lastCost;
        private double currentAmount;
        private double percent;
        private List<DataPoint> priceDynamicsPoints = new();

        private Context context = Singleton.GetObject<Context>();
        private Logger logger = Singleton.GetObject<Logger>();
        #endregion Fields

        #region Properties
        public RemainGroup RemainGroup => remain.IdGroupNavigation;
        public string Url => remain.IdSkinNavigation.Url;
        public string Title => remain.IdSkinNavigation.Title;
        public DateTime DatePurchase => datePurchase;
        public long Count => remain.Count;
        public double CostPurchase => remain.CostPurchase;
        public double AmountPurchase => amountPurchase;
        public Dictionary<DateTime, double> PriceDynamics => priceDynamics;
        public double MinPrice => minPrice;
        public double MaxPrice => maxPrice;
        public DateTime DateLastUpdate => dateLastUpdate;
        public double LastCost => lastCost;
        public double CurrentAmount => currentAmount;
        public double Percent => percent;
        public List<DataPoint> PriceDynamicsPoints => priceDynamicsPoints;
        #endregion Properties

        #region Constructor
        public RemainModel(Remain remain)
        {
            this.remain = remain;
            datePurchase = DateTime.ParseExact(remain.DatePurchase, Constants.DateTimeFormat, null);
            amountPurchase = remain.CostPurchase * remain.Count;
            context.DBContext.PriceDynamics.LoadAsync();
            context.DBContext.Skins.LoadAsync();
            UpdatePriceDynamics();
        }
        public RemainModel()
        {
            remain = new();
            context.AddRemain(remain);
        }
        #endregion Constructor

        #region Methods
        private void UpdatePriceDynamics()
        {
            priceDynamics = remain.PriceDynamics.ToDictionary(x => DateTime.ParseExact(x.DateUpdate, Constants.DateTimeFormat, null), x => x.CostUpdate);
            priceDynamics.Add(DatePurchase, CostPurchase);
            if (priceDynamics.Count == 1) priceDynamics.Add(DatePurchase.AddMilliseconds(1), CostPurchase);
            priceDynamics = priceDynamics.OrderBy(x => x.Key.Ticks).ToDictionary(x => x.Key, x => x.Value);

            minPrice = priceDynamics.Values.Min();
            maxPrice = priceDynamics.Values.Max();
            dateLastUpdate = priceDynamics.Last().Key;
            lastCost = priceDynamics.Last().Value;
            currentAmount = lastCost * Count;
            percent = (lastCost - CostPurchase) / CostPurchase * 100;

            int i = 0;
            foreach (var item in priceDynamics)
            {
                priceDynamicsPoints.Add(new(i, item.Value));
                i++;
            }
        }
        public void EditRemain(string url, long count, double costPurchase, DateTime datePurchase, RemainGroupModel? remainGroupModel)
        {
            try
            {
                var skin = context.GetSkin(url);
                if (skin is null) throw new Exception("Ссылка на скин неверна!");
                remain.IdSkinNavigation = skin;
                remain.Count = count;
                remain.CostPurchase = costPurchase;
                remain.DatePurchase = datePurchase.ToString(Constants.DateFormat);
                remain.IdGroup = remainGroupModel is null ? 1 : remainGroupModel.RemainGroup.Id;
                logger.WriteMessage($"Элемент {Title} успешно изменён!", this.GetType());
            }
            catch (Exception ex)
            {
                context.UndoChanges();
                logger.WriteMessage($"Не удалось изменить элемент {Title}. Ошибка: {ex.Message}", this.GetType());
            }
        }
        public void SellRemain(long count, double costSold, DateTime dateSold, ArchiveGroupModel? archiveGroupModel)
        {
            try
            {
                logger.WriteMessage($"Элемент {Title} успешно продан в количестве {count} штук по цене {costSold}!", this.GetType());
            }
            catch (Exception ex)
            {
                context.UndoChanges();
                logger.WriteMessage($"Не удалось продать элемент {Title}. Ошибка: {ex.Message}", this.GetType());
            }
        }
        public void DeleteRemain()
        {
            try
            {
                logger.WriteMessage($"Элемент {Title} успешно удалён!", this.GetType());
            }
            catch (Exception ex)
            {
                context.UndoChanges();
                logger.WriteMessage($"Не удалось удалить элемент {Title}. Ошибка: {ex.Message}", this.GetType());
            }
        }
        public void UpdateCurrentCost()
        {
            try
            {
                logger.WriteMessage($"Текущая цена элемент {Title} успешно добавлена!", this.GetType());
            }
            catch (Exception ex)
            {
                context.UndoChanges();
                logger.WriteMessage($"Не удалось узнать текущую цену элемента {Title}. Ошибка: {ex.Message}", this.GetType());
            }
        }
        #endregion Methods
    }
}