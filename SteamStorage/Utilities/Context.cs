using Microsoft.EntityFrameworkCore;
using SteamStorage.Entities;
using SteamStorage.Models;
using SteamStorage.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;

namespace SteamStorage.Utilities
{
    public class Context
    {
        #region Fields
        private readonly SteamStorageDbContext DbContext = new();
        private IEnumerable<RemainModel> remainModels;
        private IEnumerable<ArchiveModel> archiveModels;
        private IEnumerable<RemainGroupModel> remainGroupModels;
        private IEnumerable<ArchiveGroupModel> archiveGroupModels;

        private Logger logger = Singleton.GetObject<Logger>();
        private SteamParser parser = Singleton.GetObject<SteamParser>();
        #endregion Fields

        #region Properties
        public SteamStorageDbContext DBContext => DbContext;
        public IEnumerable<RemainGroupModel> RemainGroups => remainGroupModels;
        public IEnumerable<ArchiveGroupModel> ArchiveGroups => archiveGroupModels;
        #endregion Properties

        #region Constructor
        public Context()
        {
            UpdateRemainModels();
            UpdateArchiveModels();
            UpdateRemainGroupModels();
            UpdateArchiveGroupModels();
        }
        #endregion Constructor

        #region Methods
        public IEnumerable<RemainModel> GetRemainModels(RemainGroupModel? groupModel)
        {
            return remainModels.Where(x => groupModel is null || x.RemainGroup == groupModel.RemainGroup);
        }
        public IEnumerable<ArchiveModel> GetArchiveModels(ArchiveGroupModel? groupModel)
        {
            return archiveModels.Where(x => groupModel is null || x.ArchiveGroup == groupModel.ArchiveGroup);
        }
        public void AddRemainGroup(RemainGroup remainGroup)
        {
            DbContext.RemainGroups.Add(remainGroup);
        }
        public void AddArchiveGroup(ArchiveGroup archiveGroup)
        {
            DbContext.ArchiveGroups.Add(archiveGroup);
        }
        public void AddRemain(Remain remain)
        {
            DbContext.Remains.Add(remain);
        }
        public void AddArchive(Archive archive)
        {
            DbContext.Archives.Add(archive);
        }
        public void RemoveRemainGroup(RemainGroup remainGroup)
        {
            DbContext.RemainGroups.Remove(remainGroup);
        }
        public void RemoveArchiveGroup(ArchiveGroup archiveGroup)
        {
            DbContext.ArchiveGroups.Remove(archiveGroup);
        }
        public void RemoveRemain(Remain remain)
        {
            DbContext.Remains.Remove(remain);
        }
        public void RemoveArchive(Archive archive)
        {
            DbContext.Archives.Remove(archive);
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
                        Title = parser.GetSkinTitle(url)
                    };
                    DBContext.Skins.Add(skin);
                    SaveChanges();
                    logger.WriteMessage($"Добавление нового элемента по ссылке \"{url}\" удалось!");
                    return skin;
                }
                catch (Exception ex)
                {
                    logger.WriteMessage($"Добавление нового элемента по ссылке \"{url}\" не удалось! {ex.Message}");
                    UndoChanges();
                    return null;
                }
            }
        }
        public void AddPriceDynamic(RemainModel remainModel)
        {
            try
            {
                var (DateUpdate, Price) = parser.GetCurrentSkinInfo(remainModel.Url);
                PriceDynamic priceDynamic = new()
                {
                    IdRemainNavigation = remainModel.Remain,
                    CostUpdate = Price,
                    DateUpdate = DateUpdate.ToString(Constants.DateTimeFormat)
                };
                DbContext.PriceDynamics.Add(priceDynamic);
                SaveChanges();
                logger.WriteMessage($"Добавление новой записи PriceDynamics успешно!");
            }
            catch (Exception ex)
            {
                logger.WriteMessage($"Добавление новой записи PriceDynamics неудачно! {ex.Message}");
                UndoChanges();
            }
        }
        public void SaveChanges()
        {
            DbContext.SaveChanges();
        }
        public void UndoChanges()
        {
            foreach (var entry in DbContext.ChangeTracker.Entries())
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
            remainModels = DBContext.Remains.Select(x => new RemainModel(x));
        }
        public void UpdateArchiveModels()
        {
            archiveModels = DBContext.Archives.Select(x => new ArchiveModel(x));
        }
        public void UpdateRemainGroupModels()
        {
            remainGroupModels = DBContext.RemainGroups.Select(x => new RemainGroupModel(x));
        }
        public void UpdateArchiveGroupModels()
        {
            archiveGroupModels = DBContext.ArchiveGroups.Select(x => new ArchiveGroupModel(x));
        }
        #endregion Methods
    }
}