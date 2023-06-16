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
        private double totalArchiveAmount;
        private double totalArchivePercent;
        private long totalRemainCount;
        private double totalRemainAmount;
        private double totalRemainPercent;
        #endregion Fields

        #region Properties
        public long TotalArchiveCount => totalArchiveCount;
        public double TotalArchiveAmount => totalArchiveAmount;
        public double TotalArchivePercent => totalArchivePercent;
        public long TotalRemainCount => totalRemainCount;
        public double TotalRemainAmount => totalRemainAmount;
        public double TotalRemainPercent => totalRemainPercent;
        #endregion Properties

        #region Constructor
        public HomeVM()
        {
            var archiveModels = Context.GetArchiveModels(null);
            totalArchiveCount = Context.GetArchiveTotalCount(archiveModels);
            totalArchiveAmount = Context.GetArchiveTotalAmountPurchase(archiveModels);
            totalArchivePercent = Context.GetArchiveAveragePercent(archiveModels);

            var remainModels = Context.GetRemainModels(null);
            totalRemainCount = Context.GetRemainTotalCount(remainModels);
            totalRemainAmount = Context.GetRemainTotalAmountPurchase(remainModels);
            totalRemainPercent = Context.GetRemainAveragePercent(remainModels);
        }
        #endregion Constructor

        #region Methods
        #endregion Methods
    }
}
