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
        private List<DataPoint> priceDynamicsPoints;

        private Logger? logger = Singleton.GetObject<Logger>();
        #endregion Fields

        #region Properties
        public RemainGroup RemainGroup => remain.IdGroupNavigation;
        public string Url => remain.IdSkinNavigation.Url;
        public string Title => remain.IdSkinNavigation.Title;
        public Remain Remain => remain;
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
        public RemainElementModel(Remain remain)
        {
            this.remain = remain;
            datePurchase = DateTime.ParseExact(remain.DatePurchase, Constants.DateTimeFormat, null);
            amountPurchase = remain.CostPurchase * remain.Count;
            UpdatePriceDynamics();
        }
        public RemainElementModel()
        {
            remain = new();
            Context.AddRemain(remain);
        }
        #endregion Constructor

        #region Methods
        private void UpdatePriceDynamics()
        {
            Context.DBContext.PriceDynamics.LoadAsync();
            Context.DBContext.Skins.LoadAsync();
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
            priceDynamicsPoints = new();
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
                var skin = Context.GetSkin(url);
                if (skin is null) throw new Exception("Ссылка на скин неверна!");
                remain.IdSkinNavigation = skin;
                remain.Count = count;
                remain.CostPurchase = costPurchase;
                remain.DatePurchase = datePurchase.ToString(Constants.DateTimeFormat);
                remain.IdGroup = remainGroupModel is null ? 1 : remainGroupModel.RemainGroup.Id;
                Context.SaveChanges();
                logger?.WriteMessage($"Элемент {Title} успешно изменён!", this.GetType());
            }
            catch (Exception ex)
            {
                Context.UndoChanges();
                logger?.WriteMessage($"Не удалось изменить элемент {Title}. Ошибка: {ex.Message}", this.GetType());
            }
        }
        public void SellRemain(long count, double costSold, DateTime dateSold, ArchiveGroupModel? archiveGroupModel)
        {
            try
            {
                ArchiveElementModel archiveModel = new();
                archiveModel.EditArchive(Url, count, CostPurchase, costSold, DatePurchase, dateSold, archiveGroupModel);
                if (count >= remain.Count) Context.RemoveRemain(remain);
                EditRemain(Url, Count - count, CostPurchase, DatePurchase, Context.RemainGroups.ToList().Where(x => x.RemainGroup == RemainGroup).First());
                Context.SaveChanges();
                logger?.WriteMessage($"Элемент {Title} успешно продан в количестве {count} штук по цене {costSold}!", this.GetType());
            }
            catch (Exception ex)
            {
                Context.UndoChanges();
                logger?.WriteMessage($"Не удалось продать элемент {Title}. Ошибка: {ex.Message}", this.GetType());
            }
        }
        public void DeleteRemain()
        {
            try
            {
                Context.RemoveRemain(remain);
                Context.SaveChanges();
                logger?.WriteMessage($"Элемент {Title} успешно удалён!", this.GetType());
            }
            catch (Exception ex)
            {
                Context.UndoChanges();
                logger?.WriteMessage($"Не удалось удалить элемент {Title}. Ошибка: {ex.Message}", this.GetType());
            }
        }
        public void UpdateCurrentCost()
        {
            try
            {
                Context.AddPriceDynamic(this);
                Context.SaveChanges();
                UpdatePriceDynamics();
                logger?.WriteMessage($"Текущая цена элемента {Title} успешно добавлена!", this.GetType());
            }
            catch (Exception ex)
            {
                Context.UndoChanges();
                logger?.WriteMessage($"Не удалось узнать текущую цену элемента {Title}. Ошибка: {ex.Message}", this.GetType());
            }
        }
        #endregion Methods
    }
}