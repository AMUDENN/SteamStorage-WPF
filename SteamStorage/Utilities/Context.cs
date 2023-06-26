using Microsoft.EntityFrameworkCore;
using SteamStorage.Entities;
using SteamStorage.Models;
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
        #endregion Fields

        #region Properties
        public static SteamStorageDbContext DBContext => DbContext;
        public static IEnumerable<RemainGroupModel> RemainGroups => remainGroupModels;
        public static IEnumerable<ArchiveGroupModel> ArchiveGroups => archiveGroupModels;
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
        public static IEnumerable<RemainModel> GetRemainModels(RemainGroupModel? groupModel)
        {
            return remainModels.Where(x => groupModel is null || x.RemainGroup == groupModel.RemainGroup);
        }
        public static IEnumerable<ArchiveModel> GetArchiveModels(ArchiveGroupModel? groupModel)
        {
            return archiveModels.Where(x => groupModel is null || x.ArchiveGroup == groupModel.ArchiveGroup);
        }
        public static void AddArchiveGroup(ArchiveGroup archiveGroup)
        {
            DbContext.ArchiveGroups.Add(archiveGroup);
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