using SteamStorage.Entities;
using SteamStorage.Models;
using System.Collections;
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
        public static long GetRemainTotalCount(IEnumerable<RemainModel> remainModels) => remainModels.Sum(x => x.Count);
        public static double GetRemainTotalAmountPurchase(IEnumerable<RemainModel> remainModels) => remainModels.Sum(x => x.AmountPurchase);
        public static double GetRemainAverageCostPurchase(IEnumerable<RemainModel> remainModels)
        {
            var totalCount = GetRemainTotalCount(remainModels);
            if (totalCount == 0) return 0;
            return GetRemainTotalAmountPurchase(remainModels) / totalCount;
        }
        public static double GetRemainTotalCurrentAmount(IEnumerable<RemainModel> remainModels) => remainModels.Sum(x => x.CurrentAmount);
        public static double GetRemainAverageCurrentCost(IEnumerable<RemainModel> remainModels)
        {
            var totalCount = GetRemainTotalCount(remainModels);
            if (totalCount == 0) return 0;
            return GetRemainTotalCurrentAmount(remainModels) / totalCount;
        }
        public static double GetRemainAveragePercent(IEnumerable<RemainModel> remainModels) 
        {
            var totalAmountPurchase = GetRemainTotalAmountPurchase(remainModels);
            if (totalAmountPurchase == 0) return 0;
            return (GetRemainTotalCurrentAmount(remainModels) - totalAmountPurchase) / totalAmountPurchase * 100;
        }
        public static IEnumerable<ArchiveModel> GetArchiveModels(ArchiveGroupModel? groupModel)
        {
            return archiveModels.Where(x => groupModel is null || x.ArchiveGroup == groupModel.ArchiveGroup);
        }
        public static long GetArchiveTotalCount(IEnumerable<ArchiveModel> archiveModels) => archiveModels.Sum(x => x.Count);
        public static double GetArchiveTotalAmountPurchase(IEnumerable<ArchiveModel> archiveModels) => archiveModels.Sum(x => x.AmountPurchase);
        public static double GetArchiveAverageCostPurchase(IEnumerable<ArchiveModel> archiveModels)
        {
            var totalCount = GetArchiveTotalCount(archiveModels);
            if (totalCount == 0) return 0;
            return GetArchiveTotalAmountPurchase(archiveModels) / totalCount;
        }
        public static double GetArchiveTotalAmountSold(IEnumerable<ArchiveModel> archiveModels) => archiveModels.Sum(x => x.AmountSold);
        public static double GetArchiveAverageCostSold(IEnumerable<ArchiveModel> archiveModels)
        {
            var totalCount = GetArchiveTotalCount(archiveModels);
            if (totalCount == 0) return 0;
            return GetArchiveTotalAmountSold(archiveModels) / totalCount;
        }
        public static double GetArchiveAveragePercent(IEnumerable<ArchiveModel> archiveModels)
        {
            var totalAmountPurchase = GetArchiveTotalAmountPurchase(archiveModels);
            if (totalAmountPurchase == 0) return 0;
            return (GetArchiveTotalAmountSold(archiveModels) - totalAmountPurchase) / totalAmountPurchase * 100;
        }
        #endregion Methods
    }
}