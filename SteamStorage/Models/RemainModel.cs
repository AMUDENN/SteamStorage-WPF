using Microsoft.EntityFrameworkCore;
using OxyPlot;
using SteamStorage.Entities;
using SteamStorage.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SteamStorage.Models
{
    public class RemainModel
    {
        #region Fields
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
        private Remain remain;
        #endregion Fields

        #region Properties
        public RemainGroup RemainGroup => remain.IdGroupNavigation;
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
            Context.DBContext.PriceDynamics.LoadAsync();
            Context.DBContext.Skins.LoadAsync();
            UpdatePriceDynamics();
        }
        #endregion Constructor

        #region Methods
        public void UpdatePriceDynamics()
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
        #endregion Methods
    }
}