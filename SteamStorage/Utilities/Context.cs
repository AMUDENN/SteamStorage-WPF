using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;
using SteamStorage.Entities;
using SteamStorage.Models;
using SteamStorage.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace SteamStorage.Utilities
{
    public class Context : ObservableObject
    {
        #region Fields
        private readonly SteamStorageDbContext _dbContext = new();
        private IEnumerable<RemainElementModel> _remainElementModels;
        private IEnumerable<ArchiveElementModel> _archiveElementModels;
        private IEnumerable<RemainGroupModel> _remainGroupModels;
        private IEnumerable<ArchiveGroupModel> _archiveGroupModels;

        private readonly Logger? _logger;
        #endregion Fields

        #region Properties
        public SteamStorageDbContext DBContext => _dbContext;
        private IEnumerable<RemainElementModel> RemainElementModels
        {
            get => _remainElementModels.ToList();
            set => SetProperty(ref _remainElementModels, value);
        }
        private IEnumerable<ArchiveElementModel> ArchiveElementModels
        {
            get => _archiveElementModels.ToList();
            set => SetProperty(ref _archiveElementModels, value);
        }
        private IEnumerable<RemainGroupModel> RemainGroupModels
        {
            get => _remainGroupModels.ToList();
            set => SetProperty(ref _remainGroupModels, value);
        }
        private IEnumerable<ArchiveGroupModel> ArchiveGroupModels
        {
            get => _archiveGroupModels.ToList();
            set => SetProperty(ref _archiveGroupModels, value);
        }
        #endregion Properties

        #region Constructor
        public Context(Logger logger)
        {
            _logger = logger;
            UpdateRemainModels();
            UpdateArchiveModels();
            UpdateRemainGroupModels();
            UpdateArchiveGroupModels();
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
        public List<RemainGroupModel> GetRemainGroupModels() => RemainGroupModels.ToList();
        public List<ArchiveGroupModel> GetArchiveGroupModels() => ArchiveGroupModels.ToList();
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
        }
        public void RemoveArchiveGroup(ArchiveGroup archiveGroup)
        {
            DBContext.ArchiveGroups.Remove(archiveGroup);
        }
        public void RemoveRemain(Remain remain)
        {
            DBContext.Remains.Remove(remain);
        }
        public void RemoveArchive(Archive archive)
        {
            DBContext.Archives.Remove(archive);
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
                    _logger?.WriteMessage($"Добавление нового элемента по ссылке \"{url}\" удалось!");
                    return skin;
                }
                catch (Exception ex)
                {
                    _logger?.WriteMessage($"Добавление нового элемента по ссылке \"{url}\" не удалось! {ex.Message}");
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
                    DateUpdate = DateUpdate.ToString(Constants.DateTimeFormat)
                };
                DBContext.PriceDynamics.Add(priceDynamic);
                SaveChanges();
                _logger?.WriteMessage($"Добавление новой записи успешно!", typeof(PriceDynamic));
            }
            catch (Exception ex)
            {
                _logger?.WriteMessage($"Добавление новой записи неудачно! {ex.Message}", typeof(PriceDynamic));
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
        #endregion Methods
    }
}