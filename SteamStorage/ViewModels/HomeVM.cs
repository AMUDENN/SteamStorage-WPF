using CommunityToolkit.Mvvm.ComponentModel;
using SteamStorage.Models;
using SteamStorage.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SteamStorage.ViewModels
{
    public class HomeVM : ObservableObject
    {
        #region Fields
        private IEnumerable<ArchiveGroupModel> archiveGroupModels = Context.ArchiveGroups;
        private IEnumerable<RemainGroupModel> remainGroupModels = Context.RemainGroups;

        private long totalArchiveCount;
        private double totalArchiveAmountPurchase;
        private double totalArchivePercent;
        private double totalArchiveAmountSold;
        private long totalRemainCount;
        private double totalRemainAmountPurchase;
        private double totalRemainPercent;
        private double totalRemainCurrentAmount;
        #endregion Fields

        #region Properties
        public long TotalArchiveCount => totalArchiveCount;
        public double TotalArchiveAmountPurchase => totalArchiveAmountPurchase;
        public double TotalArchivePercent => totalArchivePercent;
        public double TotalArchiveAmountSold => totalArchiveAmountSold;
        public long TotalRemainCount => totalRemainCount;
        public double TotalRemainAmountPurchase => totalRemainAmountPurchase;
        public double TotalRemainPercent => totalRemainPercent;
        public double TotalRemainCurrentAmount => totalRemainCurrentAmount;
        #endregion Properties

        #region Constructor
        public HomeVM()
        {
            var archiveModels = Context.GetArchiveModels(null);
            totalArchiveCount = CalculationModel.GetArchiveTotalCount(archiveModels);
            totalArchiveAmountPurchase = CalculationModel.GetArchiveTotalAmountPurchase(archiveModels);
            totalArchivePercent = CalculationModel.GetArchiveAveragePercent(archiveModels);
            totalArchiveAmountSold = CalculationModel.GetArchiveTotalAmountSold(archiveModels);

            var remainModels = Context.GetRemainModels(null);
            totalRemainCount = CalculationModel.GetRemainTotalCount(remainModels);
            totalRemainAmountPurchase = CalculationModel.GetRemainTotalAmountPurchase(remainModels);
            totalRemainPercent = CalculationModel.GetRemainAveragePercent(remainModels);
            totalRemainCurrentAmount = CalculationModel.GetRemainTotalCurrentAmount(remainModels);
        }
        #endregion Constructor

        #region Methods
        #endregion Methods
    }
}
