using System.Collections.Generic;
using System.Linq;

namespace SteamStorage.Models
{
    public static class CalculationModel
    {
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
        public static RemainModel? GetMostProfitabilityRemain(IEnumerable<RemainModel> remainModels) => remainModels.MaxBy(x => x.Percent);
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
        public static ArchiveModel? GetMostProfitabilityArchive(IEnumerable<ArchiveModel> archiveModels) => archiveModels.MaxBy(x => x.Percent);
    }
}
