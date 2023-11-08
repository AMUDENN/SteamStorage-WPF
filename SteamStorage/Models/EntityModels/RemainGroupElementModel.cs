using SteamStorage.Entities;
using SteamStorage.Services.Logger;
using SteamStorage.Utilities;
using System;

namespace SteamStorage.Models.EntityModels
{
    public class RemainGroupElementModel
    {
        #region Fields
        private RemainGroup _remainGroup;
        private long? _remainsCount;
        private double? _remainsAmount;
        private double? _remainsPercent;

        private readonly LoggerService? _loggerService = Singleton.GetObject<LoggerService>();
        private readonly Context? _context = Singleton.GetObject<Context>();
        #endregion Fields

        #region Properties
        public RemainGroup RemainGroup => _remainGroup;
        public string Title => _remainGroup.Title;
        public long RemainsCount
        {
            get
            {
                if (_remainsCount is null) UpdateRemains();
                return (long)_remainsCount;
            }
        }
        public double RemainsAmount
        {
            get
            {
                if (_remainsAmount is null) UpdateRemains();
                return (double)_remainsAmount;
            }
        }
        public double RemainsPercent
        {
            get
            {
                if (_remainsPercent is null) UpdateRemains();
                return (double)_remainsPercent;
            }
        }
        #endregion Properties

        #region Constructor
        public RemainGroupElementModel(RemainGroup remainGroup)
        {
            _remainGroup = remainGroup;
        }
        public RemainGroupElementModel(string title)
        {
            _remainGroup = new();
            EditGroup(title);
            _context?.AddRemainGroup(_remainGroup);
        }
        #endregion Constructor

        #region Methods
        private void UpdateRemains()
        {
            var remainModels = _context?.GetRemainModels(this);
            _remainsCount = CalculationModel.GetRemainTotalCount(remainModels);
            _remainsAmount = CalculationModel.GetRemainTotalAmountPurchase(remainModels);
            _remainsPercent = CalculationModel.GetRemainAveragePercent(remainModels);
        }
        public void EditGroup(string title)
        {
            try
            {
                _context?.EditRemainGroup(_remainGroup, title);
                _context?.UpdateRemainGroupModels();
                _loggerService?.WriteMessage($"Группа {Title} успешно изменёна!", GetType());
            }
            catch (Exception ex)
            {
                _context?.UndoChanges();
                _loggerService?.WriteMessage($"Не удалось изменить группу {Title}. Ошибка: {ex.Message}", GetType());
                UserMessage.Error($"Не удалось изменить группу {Title}");
            }
        }
        public void DeleteGroup()
        {
            try
            {
                var remains = _context?.GetRemainModels(this);
                foreach (var item in remains)
                {
                    item.EditRemain(null);
                }
                _context?.RemoveRemainGroup(_remainGroup);
                _loggerService?.WriteMessage($"Группа {Title} успешно удалена!", GetType());
            }
            catch (Exception ex)
            {
                _context?.UndoChanges();
                _loggerService?.WriteMessage($"Не удалось удалить группу {Title}. Ошибка: {ex.Message}", GetType());
                UserMessage.Error($"Не удалось удалить группу {Title}");
            }
        }
        public void DeleteGroupWithSkins()
        {
            try
            {
                var remains = _context?.GetRemainModels(this);
                foreach (var item in remains)
                {
                    item.DeleteRemain();
                }
                _context?.RemoveRemainGroup(_remainGroup);
                _loggerService?.WriteMessage($"Группа {Title} успешно удалена вместе со скинами!", GetType());
            }
            catch (Exception ex)
            {
                _context?.UndoChanges();
                _loggerService?.WriteMessage($"Не удалось удалить группу {Title}. Ошибка: {ex.Message}", GetType());
                UserMessage.Error($"Не удалось удалить группу {Title}");
            }
        }
        #endregion Methods
    }
}
