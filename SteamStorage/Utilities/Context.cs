using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;
using SteamStorage.Entities;
using SteamStorage.Models;
using SteamStorage.Parser;
using SteamStorage.Services.Logger;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SteamStorage.Utilities
{
    public class Context : ObservableObject
    {
        #region Fields
        private readonly SteamStorageDbContext _dbContext = new();
        private readonly SteamStorageDbContext _dbContextAdditional = new();
        private IEnumerable<RemainElementModel> _remainElementModels;
        private IEnumerable<ArchiveElementModel> _archiveElementModels;
        private IEnumerable<RemainGroupModel> _remainGroupModels;
        private IEnumerable<ArchiveGroupModel> _archiveGroupModels;

        private readonly LoggerService? _loggerService;
        #endregion Fields

        #region Properties
        public SteamStorageDbContext DBContext => _dbContext;
        public SteamStorageDbContext DBContextAdditional => _dbContextAdditional;
        public IEnumerable<RemainElementModel> RemainElementModels
        {
            get => _remainElementModels.ToList();
            set => SetProperty(ref _remainElementModels, value);
        }
        public IEnumerable<ArchiveElementModel> ArchiveElementModels
        {
            get => _archiveElementModels.ToList();
            set => SetProperty(ref _archiveElementModels, value);
        }
        public IEnumerable<RemainGroupModel> RemainGroupModels
        {
            get => _remainGroupModels.ToList();
            set => SetProperty(ref _remainGroupModels, value);
        }
        public IEnumerable<ArchiveGroupModel> ArchiveGroupModels
        {
            get => _archiveGroupModels.ToList();
            set => SetProperty(ref _archiveGroupModels, value);
        }
        #endregion Properties

        #region Constructor
        public Context(LoggerService? logger)
        {
            _loggerService = logger;
            UpdateAll();
        }
        #endregion Constructor

        #region Methods
        public List<RemainElementModel> GetRemainModels(RemainGroupModel? groupModel)
        {
            return RemainElementModels.Where(x => groupModel is null || x.RemainGroup == groupModel.RemainGroup).ToList();
        }
        public List<ArchiveElementModel> GetArchiveModels(ArchiveGroupModel? groupModel)
        {
            return ArchiveElementModels.Where(x => groupModel is null || x.ArchiveGroup == groupModel.ArchiveGroup).ToList();
        }
        public void AddRemainGroup(RemainGroup remainGroup)
        {
            DBContext.RemainGroups.Add(remainGroup);
        }
        public void AddArchiveGroup(ArchiveGroup archiveGroup)
        {
            DBContext.ArchiveGroups.Add(archiveGroup);
        }
        public void AddRemain(Remain remain)
        {
            DBContext.Remains.Add(remain);
        }
        public void AddArchive(Archive archive)
        {
            DBContext.Archives.Add(archive);
        }
        public void RemoveRemainGroup(RemainGroup remainGroup)
        {
            DBContext.RemainGroups.Remove(remainGroup);
            SaveChanges();
            UpdateRemainGroupModels();
            UpdateRemainModels();
        }
        public void RemoveArchiveGroup(ArchiveGroup archiveGroup)
        {
            DBContext.ArchiveGroups.Remove(archiveGroup);
            SaveChanges();
            UpdateArchiveGroupModels();
            UpdateArchiveModels();
        }
        public void RemoveRemain(Remain remain)
        {
            DBContext.Remains.Remove(remain);
            SaveChanges();
            UpdateRemainModels();
        }
        public void RemoveArchive(Archive archive)
        {
            DBContext.Archives.Remove(archive);
            SaveChanges();
            UpdateArchiveModels();
        }
        public Skin? GetSkin(string url)
        {
            var skins = DBContext.Skins.Where(x => x.Url == url);
            if (skins.Any())
                return skins.First();
            else
            {
                try
                {
                    Skin skin = new()
                    {
                        Url = url,
                        Title = SteamParser.GetSkinTitle(url)
                    };
                    DBContext.Skins.Add(skin);
                    SaveChanges();
                    _loggerService?.WriteMessage($"Добавление нового элемента по ссылке \"{url}\" удалось!");
                    return skin;
                }
                catch (Exception ex)
                {
                    _loggerService?.WriteMessage($"Добавление нового элемента по ссылке \"{url}\" не удалось! {ex.Message}");
                    UndoChanges();
                    return null;
                }
            }
        }
        public void AddPriceDynamic(RemainElementModel remainModel)
        {
            try
            {
                var (DateUpdate, Price) = SteamParser.GetCurrentSkinInfo(remainModel.Url);
                PriceDynamic priceDynamic = new()
                {
                    IdRemainNavigation = remainModel.Remain,
                    CostUpdate = Price,
                    DateUpdate = DateUpdate.ToString(ProgramConstants.DateTimeFormat)
                };
                DBContextAdditional.PriceDynamics.Add(priceDynamic);
                SaveChanges();
                _loggerService?.WriteMessage($"Добавление новой записи успешно!", typeof(PriceDynamic));
            }
            catch (Exception ex)
            {
                _loggerService?.WriteMessage($"Добавление новой записи неудачно! {ex.Message}", typeof(PriceDynamic));
                UndoChanges();
            }
        }
        public void SaveChanges()
        {
            DBContext.SaveChanges();
        }
        public void UndoChanges()
        {
            foreach (var entry in DBContext.ChangeTracker.Entries())
            {
                switch (entry.State)
                {
                    case EntityState.Modified:
                        entry.State = EntityState.Unchanged;
                        break;
                    case EntityState.Deleted:
                        entry.Reload();
                        break;
                    case EntityState.Added:
                        entry.State = EntityState.Detached;
                        break;
                }
            }
        }
        private void UpdateAll()
        {
            UpdateRemainModels();
            UpdateArchiveModels();
            UpdateRemainGroupModels();
            UpdateArchiveGroupModels();
        }
        public void UpdateRemainModels()
        {
            RemainElementModels = DBContext.Remains.Select(x => new RemainElementModel(x));
        }
        public void UpdateArchiveModels()
        {
            ArchiveElementModels = DBContext.Archives.Select(x => new ArchiveElementModel(x));
        }
        public void UpdateRemainGroupModels()
        {
            RemainGroupModels = DBContext.RemainGroups.Select(x => new RemainGroupModel(x));
        }
        public void UpdateArchiveGroupModels()
        {
            ArchiveGroupModels = DBContext.ArchiveGroups.Select(x => new ArchiveGroupModel(x));
        }
        public void ClearDatabase()
        {
            try
            {
                DBContext.Archives.RemoveRange(DBContext.Archives);
                DBContext.ArchiveGroups.RemoveRange(DBContext.ArchiveGroups.Skip(1));
                DBContext.PriceDynamics.RemoveRange(DBContext.PriceDynamics);
                DBContext.Remains.RemoveRange(DBContext.Remains);
                DBContext.RemainGroups.RemoveRange(DBContext.RemainGroups.Skip(1));
                DBContext.Skins.RemoveRange(DBContext.Skins);
                SaveChanges();
                UpdateAll();
                _loggerService?.WriteMessage("Очистка базы данных прошла успешно!", typeof(Context));
            }
            catch
            {
                _loggerService?.WriteMessage("Очистка базы данных прошла неудачно! ", typeof(Context));
                UndoChanges();
            }
        }
        #endregion Methods
    }
}