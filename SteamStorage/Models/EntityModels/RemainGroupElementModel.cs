using SteamStorage.Entities;
using SteamStorage.Services.Logger;
using SteamStorage.Utilities;
using System;
using System.Security.Policy;

namespace SteamStorage.Models.EntityModels
{
    public class RemainGroupElementModel
    {
        #region Fields
        private RemainGroup _remainGroup;
        private long? _remainsCount;
        private double? _remainsAmount;
        private double? _remainsPercent;

        private readonly LoggerService? _loggerService = Singleton.GetService<LoggerService>();
        private readonly Context? _context = Singleton.GetService<Context>();
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
            AddGroup(title);
        }
        #endregion Constructor

        #region Methods
        private void AddGroup(string title)
        {
            try
            {
                _remainGroup = new();
                _context?.EditRemainGroup(_remainGroup, title);
                _context?.AddRemainGroup(_remainGroup);
                _loggerService?.WriteMessage($"Группа {Title} успешно добавлена!", GetType());
            }
            catch (Exception ex)
            {
                _context?.UndoChanges();
                _loggerService?.WriteMessage(ex, "Добавление новой группы не удалось");
                UserMessage.Error("Добавление новой группы не удалось");
            }
        }
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
                _loggerService?.WriteMessage(ex, $"Не удалось изменить группу {Title}");
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
                _loggerService?.WriteMessage(ex, $"Не удалось удалить группу {Title}");
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
                _loggerService?.WriteMessage(ex, $"Не удалось удалить группу {Title}");
                UserMessage.Error($"Не удалось удалить группу {Title}");
            }
        }
        #endregion Methods
    }
}
