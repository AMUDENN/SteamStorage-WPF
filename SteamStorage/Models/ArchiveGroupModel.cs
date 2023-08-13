using SteamStorage.Entities;
using SteamStorage.Utilities;
using System;

namespace SteamStorage.Models
{
    public class ArchiveGroupModel
    {
        #region Fields
        private ArchiveGroup _archiveGroup;
        private long? _archivesCount;
        private double? _archivesAmount;
        private double? _archivesPercent;

        private readonly Logger? _logger = Singleton.GetObject<Logger>();
        private readonly Context? _context = Singleton.GetObject<Context>();
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
        public ArchiveGroupModel(ArchiveGroup archiveGroup)
        {
            this._archiveGroup = archiveGroup;
        }
        public ArchiveGroupModel()
        {
            _archiveGroup = new();
            _context?.AddArchiveGroup(_archiveGroup);
        }
        #endregion Constructor

        #region Methods
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
                _archiveGroup.Title = title;
                _context?.SaveChanges();
                _logger?.WriteMessage($"Группа {Title} успешно изменёна!", this.GetType());
            }
            catch (Exception ex)
            {
                _context?.UndoChanges();
                _logger?.WriteMessage($"Не удалось изменить группу {Title}. Ошибка: {ex.Message}", this.GetType());
            }
        }
        public void DeleteGroup()
        {
            try
            {
                _context?.RemoveArchiveGroup(_archiveGroup);
                _context?.SaveChanges();
                _logger?.WriteMessage($"Группа {Title} успешно удалена!", this.GetType());
            }
            catch (Exception ex)
            {
                _context?.UndoChanges();
                _logger?.WriteMessage($"Не удалось удалить группу {Title}. Ошибка: {ex.Message}", this.GetType());
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
                _context?.SaveChanges();
                _logger?.WriteMessage($"Группа {Title} успешно удалена вместе со скинами!", this.GetType());
            }
            catch (Exception ex)
            {
                _context?.UndoChanges();
                _logger?.WriteMessage($"Не удалось удалить группу {Title}. Ошибка: {ex.Message}", this.GetType());
            }
        }
        #endregion Methods
    }
}
