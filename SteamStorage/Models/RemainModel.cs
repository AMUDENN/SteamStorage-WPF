using Microsoft.EntityFrameworkCore;
using SteamStorage.Entities;
using SteamStorage.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SteamStorage.Models
{
    public class RemainModel
    {
        private DateTime datePurchase;
        private double amountPurchase;
        private Dictionary<DateTime, double> priceDynamics;
        private DateTime dateLastUpdate;
        private double lastCost;
        private double percent;
        private Remain remain;
        public RemainGroup RemainGroup => remain.IdGroupNavigation;
        public string Title => remain.IdSkinNavigation.Title;
        public DateTime DatePurchase => datePurchase;
        public long Count => remain.Count;
        public double CostPurchase => remain.CostPurchase;
        public double AmountPurchase => amountPurchase;
        public Dictionary<DateTime, double> PriceDynamics => priceDynamics;
        public DateTime DateLastUpdate => dateLastUpdate;
        public double LastCost => lastCost;
        public double Percent => percent;
        public RemainModel(Remain remain)
        {
            this.remain = remain;
            datePurchase = DateTime.ParseExact(remain.DatePurchase, Constants.DateTimeFormat, null);
            amountPurchase = remain.CostPurchase * remain.Count;
            Context.GetContext().PriceDynamics.LoadAsync();
            Context.GetContext().Skins.LoadAsync();
            UpdatePriceDynamics();
        }
        public void UpdatePriceDynamics()
        {
            priceDynamics = remain.PriceDynamics.ToDictionary(x => DateTime.ParseExact(x.DateUpdate, Constants.DateTimeFormat, null), x => x.CostUpdate);
            priceDynamics.Add(DatePurchase, CostPurchase);
            if (priceDynamics.Count == 1) priceDynamics.Add(DatePurchase.AddMilliseconds(1), CostPurchase);
            priceDynamics = priceDynamics.OrderBy(x => x.Key.Ticks).ToDictionary(x => x.Key, x => x.Value);

            dateLastUpdate = priceDynamics.Last().Key;
            lastCost = priceDynamics.Last().Value;
            percent = Math.Round((lastCost - CostPurchase) / CostPurchase * 100, 2);
        }
    }
}