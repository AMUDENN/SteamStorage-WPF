﻿using Microsoft.EntityFrameworkCore;
using SteamStorage.Entities;
using SteamStorage.Utilities;
using System;

namespace SteamStorage.Models
{
    public class ArchiveModel
    {
        #region Fields
        private Archive archive;
        private DateTime datePurchase;
        private DateTime dateSold;
        private double amountPurchase;
        private double amountSold;
        private double percent;

        private Context context = Singleton.GetObject<Context>();
        private Logger logger = Singleton.GetObject<Logger>();
        #endregion Fields

        #region Properties
        public ArchiveGroup ArchiveGroup => archive.IdGroupNavigation;
        public string Url => archive.IdSkinNavigation.Url;
        public string Title => archive.IdSkinNavigation.Title;
        public DateTime DatePurchase => datePurchase;
        public DateTime DateSold => dateSold;
        public long Count => archive.Count;
        public double CostPurchase => archive.CostPurchase;
        public double CostSold => archive.CostSold;
        public double AmountPurchase => amountPurchase;
        public double AmountSold => amountSold;
        public double Percent => percent;
        #endregion Properties

        #region Constructor
        public ArchiveModel(Archive archive)
        {
            this.archive = archive;
            datePurchase = DateTime.ParseExact(this.archive.DatePurchase, Constants.DateTimeFormat, null);
            dateSold = DateTime.ParseExact(this.archive.DateSold, Constants.DateTimeFormat, null);
            amountPurchase = archive.CostPurchase * archive.Count;
            amountSold = archive.CostSold * archive.Count;
            percent = (CostSold - CostPurchase) / CostPurchase * 100;
            context.DBContext.Skins.LoadAsync();
        }
        public ArchiveModel()
        {
            archive = new();
            context.AddArchive(archive);
        }
        #endregion Constructor

        #region Methods
        public void EditArchive(string url, long count, double costPurchase, double costSold, DateTime datePurchase, DateTime dateSold, ArchiveGroupModel? archiveGroupModel)
        {
            try
            {
                var skin = context.GetSkin(url);
                if (skin is null) throw new Exception("Ссылка на скин неверна!");
                archive.IdSkinNavigation = skin;
                archive.Count = count;
                archive.CostPurchase = costPurchase;
                archive.CostSold = costSold;
                archive.DatePurchase = datePurchase.ToString(Constants.DateTimeFormat);
                archive.DateSold = dateSold.ToString(Constants.DateTimeFormat);
                archive.IdGroup = archiveGroupModel is null ? 1 : archiveGroupModel.ArchiveGroup.Id;
                context.SaveChanges();
                logger.WriteMessage($"Элемент {Title} успешно изменён!", this.GetType());
            }
            catch (Exception ex)
            {
                context.UndoChanges();
                logger.WriteMessage($"Не удалось изменить элемент {Title}. Ошибка: {ex.Message}", this.GetType());
            }
        }
        public void DeleteArchive()
        {
            try
            {
                context.RemoveArchive(archive);
                context.SaveChanges();
                logger.WriteMessage($"Элемент {Title} успешно удалён!", this.GetType());
            }
            catch (Exception ex)
            {
                context.UndoChanges();
                logger.WriteMessage($"Не удалось удалить элемент {Title}. Ошибка: {ex.Message}", this.GetType());
            }
        }
        #endregion Methods
    }
}
