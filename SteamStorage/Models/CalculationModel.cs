using System.Collections.Generic;
using System.Linq;

namespace SteamStorage.Models
{
    public static class CalculationModel
    {
        public static long GetRemainTotalCount(IEnumerable<RemainElementModel> remainModels) => remainModels.Sum(x => x.Count);
        public static double GetRemainTotalAmountPurchase(IEnumerable<RemainElementModel> remainModels) => remainModels.Sum(x => x.AmountPurchase);
        public static double GetRemainAverageCostPurchase(IEnumerable<RemainElementModel> remainModels)
        {
            var totalCount = GetRemainTotalCount(remainModels);
            if (totalCount == 0) return 0;
            return GetRemainTotalAmountPurchase(remainModels) / totalCount;
        }
        public static double GetRemainTotalCurrentAmount(IEnumerable<RemainElementModel> remainModels) => remainModels.Sum(x => x.CurrentAmount);
        public static double GetRemainAverageCurrentCost(IEnumerable<RemainElementModel> remainModels)
        {
            var totalCount = GetRemainTotalCount(remainModels);
            if (totalCount == 0) return 0;
            return GetRemainTotalCurrentAmount(remainModels) / totalCount;
        }
        public static double GetRemainAveragePercent(IEnumerable<RemainElementModel> remainModels)
        {
            var totalAmountPurchase = GetRemainTotalAmountPurchase(remainModels);
            if (totalAmountPurchase == 0) return 0;
            return (GetRemainTotalCurrentAmount(remainModels) - totalAmountPurchase) / totalAmountPurchase * 100;
        }
        public static RemainElementModel? GetMostProfitabilityRemain(IEnumerable<RemainElementModel> remainModels) => remainModels.MaxBy(x => x.Percent);
        public static long GetArchiveTotalCount(IEnumerable<ArchiveElementModel> archiveModels) => archiveModels.Sum(x => x.Count);
        public static double GetArchiveTotalAmountPurchase(IEnumerable<ArchiveElementModel> archiveModels) => archiveModels.Sum(x => x.AmountPurchase);
        public static double GetArchiveAverageCostPurchase(IEnumerable<ArchiveElementModel> archiveModels)
        {
            var totalCount = GetArchiveTotalCount(archiveModels);
            if (totalCount == 0) return 0;
            return GetArchiveTotalAmountPurchase(archiveModels) / totalCount;
        }
        public static double GetArchiveTotalAmountSold(IEnumerable<ArchiveElementModel> archiveModels) => archiveModels.Sum(x => x.AmountSold);
        public static double GetArchiveAverageCostSold(IEnumerable<ArchiveElementModel> archiveModels)
        {
            var totalCount = GetArchiveTotalCount(archiveModels);
            if (totalCount == 0) return 0;
            return GetArchiveTotalAmountSold(archiveModels) / totalCount;
        }
        public static double GetArchiveAveragePercent(IEnumerable<ArchiveElementModel> archiveModels)
        {
            var totalAmountPurchase = GetArchiveTotalAmountPurchase(archiveModels);
            if (totalAmountPurchase == 0) return 0;
            return (GetArchiveTotalAmountSold(archiveModels) - totalAmountPurchase) / totalAmountPurchase * 100;
        }
        public static ArchiveElementModel? GetMostProfitabilityArchive(IEnumerable<ArchiveElementModel> archiveModels) => archiveModels.MaxBy(x => x.Percent);
    }
}
