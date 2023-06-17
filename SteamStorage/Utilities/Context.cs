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
        private static IEnumerable<RemainModel> remainModels = DBContext.Remains.Select(x => new RemainModel(x));
        private static IEnumerable<ArchiveModel> archiveModels = DBContext.Archives.Select(x => new ArchiveModel(x));
        private static IEnumerable<RemainGroupModel> remainGroupModels = DBContext.RemainGroups.Select(x => new RemainGroupModel(x));
        private static IEnumerable<ArchiveGroupModel> archiveGroupModels = DBContext.ArchiveGroups.Select(x => new ArchiveGroupModel(x));
        #endregion Fields

        #region Properties
        public static SteamStorageDbContext DBContext => DbContext;
        public static IEnumerable<RemainGroupModel> RemainGroups => remainGroupModels;
        public static IEnumerable<ArchiveGroupModel> ArchiveGroups => archiveGroupModels;
        #endregion Properties

        #region Methods
        public static IEnumerable<RemainModel> GetRemainModels(RemainGroupModel? groupModel)
        {
            return remainModels.Where(x => groupModel is null || x.RemainGroup == groupModel.RemainGroup);
        }
        public static IEnumerable<ArchiveModel> GetArchiveModels(ArchiveGroupModel? groupModel)
        {
            return archiveModels.Where(x => groupModel is null || x.ArchiveGroup == groupModel.ArchiveGroup);
        }
        #endregion Methods
    }
}