using SteamStorage.Entities;
using SteamStorage.Utilities;
using System;

namespace SteamStorage.Models
{
    public class RemainGroupModel
    {
        #region Fields
        private RemainGroup remainGroup;
        private long? remainsCount;
        private double? remainsAmount;
        private double? remainsPercent;

        private Context context = Singleton.GetObject<Context>();
        private Logger logger = Singleton.GetObject<Logger>();
        #endregion Fields

        #region Properties
        public RemainGroup? RemainGroup => remainGroup;
        public string Title => remainGroup.Title;
        public long RemainsCount
        {
            get
            {
                if (remainsCount is null) UpdateRemains();
                return (long)remainsCount;
            }
        }
        public double RemainsAmount
        {
            get
            {
                if (remainsAmount is null) UpdateRemains();
                return (double)remainsAmount;
            }
        }
        public double RemainsPercent
        {
            get
            {
                if (remainsPercent is null) UpdateRemains();
                return (double)remainsPercent;
            }
        }
        #endregion Properties

        #region Constructor
        public RemainGroupModel(RemainGroup remainGroup)
        {
            this.remainGroup = remainGroup;
        }
        public RemainGroupModel()
        {
            remainGroup = new();
            context.AddRemainGroup(remainGroup);
        }
        #endregion Constructor

        #region Methods
        private void UpdateRemains()
        {
            var remainModels = context.GetRemainModels(this);
            remainsCount = CalculationModel.GetRemainTotalCount(remainModels);
            remainsAmount = CalculationModel.GetRemainTotalAmountPurchase(remainModels);
            remainsPercent = CalculationModel.GetRemainAveragePercent(remainModels);
        }
        public void EditGroup(string title)
        {
            try
            {
                remainGroup.Title = title;
                logger.WriteMessage($"Группа {Title} успешно изменёна!", this.GetType());
            }
            catch (Exception ex)
            {
                context.UndoChanges();
                logger.WriteMessage($"Не удалось изменить группу {Title}. Ошибка: {ex.Message}", this.GetType());
            }
        }
        public void DeleteGroup()
        {
            try
            {
                logger.WriteMessage($"Группа {Title} успешно удалена!", this.GetType());
            }
            catch (Exception ex)
            {
                context.UndoChanges();
                logger.WriteMessage($"Не удалось удалить группу {Title}. Ошибка: {ex.Message}", this.GetType());
            }
        }
        public void DeletGroupWithSkins()
        {
            try
            {
                logger.WriteMessage($"Группа {Title} успешно удалена вместе со скинами!", this.GetType());
            }
            catch (Exception ex)
            {
                context.UndoChanges();
                logger.WriteMessage($"Не удалось удалить группу {Title}. Ошибка: {ex.Message}", this.GetType());
            }
        }
        #endregion Methods
    }
}
