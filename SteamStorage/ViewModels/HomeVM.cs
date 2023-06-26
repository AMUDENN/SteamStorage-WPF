using CommunityToolkit.Mvvm.ComponentModel;
using SteamStorage.Models;
using SteamStorage.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace SteamStorage.ViewModels
{
    public class HomeVM : ObservableObject
    {
        #region Fields
        private List<ArchiveGroupModel> archiveGroupModels = Singleton.GetObject<Context>().ArchiveGroups.ToList();
        private List<RemainGroupModel> remainGroupModels = Singleton.GetObject<Context>().RemainGroups.ToList();

        private long totalArchiveCount;
        private double totalArchiveAmountPurchase;
        private double totalArchivePercent;
        private double totalArchiveAmountSold;
        private ArchiveModel mostProfitabilityArchive;

        private long totalRemainCount;
        private double totalRemainAmountPurchase;
        private double totalRemainPercent;
        private double totalRemainCurrentAmount;
        private RemainModel mostProfitabilityRemain;

        private Context context = Singleton.GetObject<Context>();
        #endregion Fields

        #region Properties
        public List<ArchiveGroupModel> ArchiveGroupModels => archiveGroupModels;
        public List<RemainGroupModel> RemainGroupModels => remainGroupModels;
        public ArchiveGroupModel MostProfitabilityArchiveGroup => archiveGroupModels.MaxBy(x => x.ArchivesPercent);
        public RemainGroupModel MostProfitabilityRemainGroup => remainGroupModels.MaxBy(x => x.RemainsPercent);
        public long TotalArchiveCount => totalArchiveCount;
        public double TotalArchiveAmountPurchase => totalArchiveAmountPurchase;
        public double TotalArchivePercent => totalArchivePercent;
        public double TotalArchiveAmountSold => totalArchiveAmountSold;
        public ArchiveModel MostProfitabilityArchive => mostProfitabilityArchive;
        public long TotalRemainCount => totalRemainCount;
        public double TotalRemainAmountPurchase => totalRemainAmountPurchase;
        public double TotalRemainPercent => totalRemainPercent;
        public double TotalRemainCurrentAmount => totalRemainCurrentAmount;
        public RemainModel MostProfitabilityRemain => mostProfitabilityRemain;
        #endregion Properties

        #region Constructor
        public HomeVM()
        {
            var archiveModels = context.GetArchiveModels(null);
            totalArchiveCount = CalculationModel.GetArchiveTotalCount(archiveModels);
            totalArchiveAmountPurchase = CalculationModel.GetArchiveTotalAmountPurchase(archiveModels);
            totalArchivePercent = CalculationModel.GetArchiveAveragePercent(archiveModels);
            totalArchiveAmountSold = CalculationModel.GetArchiveTotalAmountSold(archiveModels);
            mostProfitabilityArchive = CalculationModel.GetMostProfitabilityArchive(archiveModels);

            var remainModels = context.GetRemainModels(null);
            totalRemainCount = CalculationModel.GetRemainTotalCount(remainModels);
            totalRemainAmountPurchase = CalculationModel.GetRemainTotalAmountPurchase(remainModels);
            totalRemainPercent = CalculationModel.GetRemainAveragePercent(remainModels);
            totalRemainCurrentAmount = CalculationModel.GetRemainTotalCurrentAmount(remainModels);
            mostProfitabilityRemain = CalculationModel.GetMostProfitabilityRemain(remainModels);
        }
        #endregion Constructor

        #region Methods
        #endregion Methods
    }
}
