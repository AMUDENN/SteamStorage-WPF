using Microsoft.EntityFrameworkCore;
using SteamStorage.Entities;
using SteamStorage.Models;
using SteamStorage.Parser;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SteamStorage.Utilities
{
    public static class Context
    {
        #region Fields
        private static readonly SteamStorageDbContext DbContext = new();
        private static IEnumerable<RemainModel> remainModels;
        private static IEnumerable<ArchiveModel> archiveModels;
        private static IEnumerable<RemainGroupModel> remainGroupModels;
        private static IEnumerable<ArchiveGroupModel> archiveGroupModels;

        private static Logger? logger = Singleton.GetObject<Logger>();
        #endregion Fields

        #region Properties
        public static SteamStorageDbContext DBContext => DbContext;
        public static List<RemainGroupModel> RemainGroups => remainGroupModels.ToList();
        public static List<ArchiveGroupModel> ArchiveGroups => archiveGroupModels.ToList();
        #endregion Properties

        #region Constructor
        static Context()
        {
            UpdateRemainModels();
            UpdateArchiveModels();
            UpdateRemainGroupModels();
            UpdateArchiveGroupModels();
        }
        #endregion Constructor

        #region Methods
        public static List<RemainModel> GetRemainModels(RemainGroupModel? groupModel)
        {
            return remainModels.Where(x => groupModel is null || x.RemainGroup == groupModel.RemainGroup).ToList();
        }
        public static List<ArchiveModel> GetArchiveModels(ArchiveGroupModel? groupModel)
        {
            return archiveModels.Where(x => groupModel is null || x.ArchiveGroup == groupModel.ArchiveGroup).ToList();
        }
        public static void AddRemainGroup(RemainGroup remainGroup)
        {
            DbContext.RemainGroups.Add(remainGroup);
        }
        public static void AddArchiveGroup(ArchiveGroup archiveGroup)
        {
            DbContext.ArchiveGroups.Add(archiveGroup);
        }
        public static void AddRemain(Remain remain)
        {
            DbContext.Remains.Add(remain);
        }
        public static void AddArchive(Archive archive)
        {
            DbContext.Archives.Add(archive);
        }
        public static void RemoveRemainGroup(RemainGroup remainGroup)
        {
            DbContext.RemainGroups.Remove(remainGroup);
        }
        public static void RemoveArchiveGroup(ArchiveGroup archiveGroup)
        {
            DbContext.ArchiveGroups.Remove(archiveGroup);
        }
        public static void RemoveRemain(Remain remain)
        {
            DbContext.Remains.Remove(remain);
        }
        public static void RemoveArchive(Archive archive)
        {
            DbContext.Archives.Remove(archive);
        }
        public static Skin? GetSkin(string url)
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
                    logger?.WriteMessage($"Добавление нового элемента по ссылке \"{url}\" удалось!");
                    return skin;
                }
                catch (Exception ex)
                {
                    logger?.WriteMessage($"Добавление нового элемента по ссылке \"{url}\" не удалось! {ex.Message}");
                    UndoChanges();
                    return null;
                }
            }
        }
        public static void AddPriceDynamic(RemainModel remainModel)
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
                DbContext.PriceDynamics.Add(priceDynamic);
                SaveChanges();
                logger?.WriteMessage($"Добавление новой записи успешно!", typeof(PriceDynamic));
            }
            catch (Exception ex)
            {
                logger?.WriteMessage($"Добавление новой записи неудачно! {ex.Message}", typeof(PriceDynamic));
                UndoChanges();
            }
        }
        public static void SaveChanges()
        {
            DbContext.SaveChanges();
        }
        public static void UndoChanges()
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
        public static void UpdateRemainModels()
        {
            remainModels = DBContext.Remains.Select(x => new RemainModel(x));
        }
        public static void UpdateArchiveModels()
        {
            archiveModels = DBContext.Archives.Select(x => new ArchiveModel(x));
        }
        public static void UpdateRemainGroupModels()
        {
            remainGroupModels = DBContext.RemainGroups.Select(x => new RemainGroupModel(x));
        }
        public static void UpdateArchiveGroupModels()
        {
            archiveGroupModels = DBContext.ArchiveGroups.Select(x => new ArchiveGroupModel(x));
        }
        #endregion Methods
    }
}