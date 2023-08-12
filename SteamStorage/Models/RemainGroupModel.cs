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

        private Logger? logger = Singleton.GetObject<Logger>();
        #endregion Fields

        #region Properties
        public RemainGroup RemainGroup => remainGroup;
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
            Context.AddRemainGroup(remainGroup);
        }
        #endregion Constructor

        #region Methods
        private void UpdateRemains()
        {
            var remainModels = Context.GetRemainModels(this);
            remainsCount = CalculationModel.GetRemainTotalCount(remainModels);
            remainsAmount = CalculationModel.GetRemainTotalAmountPurchase(remainModels);
            remainsPercent = CalculationModel.GetRemainAveragePercent(remainModels);
        }
        public void EditGroup(string title)
        {
            try
            {
                remainGroup.Title = title;
                Context.SaveChanges();
                logger?.WriteMessage($"Группа {Title} успешно изменёна!", this.GetType());
            }
            catch (Exception ex)
            {
                Context.UndoChanges();
                logger?.WriteMessage($"Не удалось изменить группу {Title}. Ошибка: {ex.Message}", this.GetType());
            }
        }
        public void DeleteGroup()
        {
            try
            {
                Context.RemoveRemainGroup(remainGroup);
                Context.SaveChanges();
                logger?.WriteMessage($"Группа {Title} успешно удалена!", this.GetType());
            }
            catch (Exception ex)
            {
                Context.UndoChanges();
                logger?.WriteMessage($"Не удалось удалить группу {Title}. Ошибка: {ex.Message}", this.GetType());
            }
        }
        public void DeleteGroupWithSkins()
        {
            try
            {
                var remains = Context.GetRemainModels(this);
                foreach (var item in remains)
                {
                    item.DeleteRemain();
                }
                Context.RemoveRemainGroup(remainGroup);
                Context.SaveChanges();
                logger?.WriteMessage($"Группа {Title} успешно удалена вместе со скинами!", this.GetType());
            }
            catch (Exception ex)
            {
                Context.UndoChanges();
                logger?.WriteMessage($"Не удалось удалить группу {Title}. Ошибка: {ex.Message}", this.GetType());
            }
        }
        #endregion Methods
    }
}
