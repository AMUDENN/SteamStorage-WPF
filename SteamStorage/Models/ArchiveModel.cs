﻿using Microsoft.EntityFrameworkCore;
using SteamStorage.Entities;
using SteamStorage.Utilities;
using System;

namespace SteamStorage.Models
{
    public class ArchiveModel
    {
        private DateTime datePurchase;
        private DateTime dateSold;
        private double amountPurchase;
        private double amountSold;
        private double percent;
        private Archive archive;
        public ArchiveGroup ArchiveGroup => archive.IdGroupNavigation;
        public string Title => archive.IdSkinNavigation.Title;
        public DateTime DatePurchase => datePurchase;
        public DateTime DateSold => dateSold;
        public long Count => archive.Count;
        public double CostPurchase => archive.CostPurchase;
        public double CostSold => archive.CostSold;
        public double AmountPurchase => amountPurchase;
        public double AmountSold => amountSold;
        public double Percent => percent;
        public ArchiveModel(Archive archive)
        {
            this.archive = archive;
            datePurchase = DateTime.ParseExact(this.archive.DatePurchase, Constants.DateFormat, null);
            dateSold = DateTime.ParseExact(this.archive.DateSold, Constants.DateFormat, null);
            amountPurchase = archive.CostPurchase * archive.Count;
            amountSold = archive.CostSold * archive.Count;
            percent = Math.Round((CostSold - CostPurchase) / CostPurchase * 100, 2);
            Context.GetContext().Skins.LoadAsync();
        }
    }
}