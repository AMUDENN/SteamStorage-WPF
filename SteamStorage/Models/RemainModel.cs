﻿using SteamStorage.Entities;
using SteamStorage.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SteamStorage.Models
{
    public class RemainModel
    {
        private readonly DateTime datePurchase;
        private readonly double amountPurchase;
        private Dictionary<DateTime, double> priceDynamics;
        private DateTime lastUpdate;
        private double lastCost;
        private double percent;
        private readonly Remain remain;
        public string Title => remain.IdSkinNavigation.Title;
        public DateTime DatePurchase => datePurchase;
        public long Count => remain.Count;
        public double CostPurchase => remain.CostPurchase;
        public double AmountPurchase => amountPurchase;
        public Dictionary<DateTime, double> PriceDynamics => priceDynamics;
        public DateTime LastUpdate => lastUpdate;
        public double LastCost => lastCost;
        public double Percent => percent;
        public RemainModel(Remain remain)
        {
            this.remain = remain;
            datePurchase = DateTime.ParseExact(this.remain.DatePurchase, Constants.DateFormat, null);
            amountPurchase = remain.CostPurchase * remain.Count;
            UpdatePriceDynamics();
        }
        public void UpdatePriceDynamics()
        {
            priceDynamics = remain.PriceDynamics.ToDictionary(x => DateTime.ParseExact(x.DateUpdate, Constants.DateFormat, null), x => x.CostUpdate);
            priceDynamics.Add(DatePurchase, CostPurchase);
            if (priceDynamics.Count == 1) priceDynamics.Add(DatePurchase.AddMilliseconds(1), CostPurchase);
            priceDynamics = (Dictionary<DateTime, double>)priceDynamics.OrderBy(x => x.Key.Ticks);

            lastUpdate = priceDynamics.Last().Key;
            lastCost = priceDynamics.Last().Value;
            percent = Math.Round((lastCost - CostPurchase) / CostPurchase * 100, 2);
        }
    }
}