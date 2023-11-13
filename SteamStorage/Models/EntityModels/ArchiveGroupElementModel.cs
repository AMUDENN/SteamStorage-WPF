using SteamStorage.Entities;
using SteamStorage.Services.Logger;
using SteamStorage.Utilities;
using System;

namespace SteamStorage.Models.EntityModels
{
    public class ArchiveGroupElementModel
    {
        #region Fields
        private ArchiveGroup _archiveGroup;
        private long? _archivesCount;
        private double? _archivesAmount;
        private double? _archivesPercent;

        private readonly LoggerService? _loggerService = Singleton.GetService<LoggerService>();
        private readonly Context? _context = Singleton.GetService<Context>();
        #endregion Fields

        #region Properties
        public ArchiveGroup ArchiveGroup => _archiveGroup;
        public string Title => _archiveGroup.Title;
        public long ArchivesCount
        {
            get
            {
                if (_archivesCount is null) UpdateArchives();
                return (long)_archivesCount;
            }
        }
        public double ArchivesAmount
        {
            get
            {
                if (_archivesAmount is null) UpdateArchives();
                return (double)_archivesAmount;
            }
        }
        public double ArchivesPercent
        {
            get
            {
                if (_archivesPercent is null) UpdateArchives();
                return (double)_archivesPercent;
            }
        }
        #endregion Properties

        #region Constructor
        public ArchiveGroupElementModel(ArchiveGroup archiveGroup)
        {
            _archiveGroup = archiveGroup;
        }
        public ArchiveGroupElementModel(string title)
        {
            AddGroup(title);
        }
        #endregion Constructor

        #region Methods
        private void AddGroup(string title)
        {
            try
            {
                _archiveGroup = new();
                _context?.EditArchiveGroup(_archiveGroup, title);
                _context?.AddArchiveGroup(_archiveGroup);
                _loggerService?.WriteMessage($"Группа {Title} успешно добавлена!", GetType());
            }
            catch (Exception ex)
            {
                _context?.UndoChanges();
                _loggerService?.WriteMessage(ex, "Добавление новой группы не удалось");
                UserMessage.Error("Добавление новой группы не удалось");
            }
        }
        private void UpdateArchives()
        {
            var archiveModels = _context?.GetArchiveModels(this);
            _archivesCount = CalculationModel.GetArchiveTotalCount(archiveModels);
            _archivesAmount = CalculationModel.GetArchiveTotalAmountPurchase(archiveModels);
            _archivesPercent = CalculationModel.GetArchiveAveragePercent(archiveModels);
        }
        public void EditGroup(string title)
        {
            try
            {
                _context?.EditArchiveGroup(_archiveGroup, title);
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
                var archives = _context?.GetArchiveModels(this);
                foreach (var item in archives)
                {
                    item.EditArchive(null);
                }
                _context?.RemoveArchiveGroup(_archiveGroup);
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
                var archives = _context?.GetArchiveModels(this);
                foreach (var item in archives)
                {
                    item.DeleteArchive();
                }
                _context?.RemoveArchiveGroup(_archiveGroup);
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
