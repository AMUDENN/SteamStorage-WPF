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
        private List<ArchiveGroupModel>? _archiveGroupModels;
        private List<RemainGroupModel>? _remainGroupModels;

        private long _totalArchiveCount;
        private double _totalArchiveAmountPurchase;
        private double _totalArchivePercent;
        private double _totalArchiveAmountSold;
        private ArchiveElementModel? _mostProfitabilityArchive;

        private long _totalRemainCount;
        private double _totalRemainAmountPurchase;
        private double _totalRemainPercent;
        private double _totalRemainCurrentAmount;
        private RemainElementModel? _mostProfitabilityRemain;

        private readonly Context? _context = Singleton.GetObject<Context>();
        #endregion Fields

        #region Properties
        public List<ArchiveGroupModel>? ArchiveGroupModels => _archiveGroupModels;
        public List<RemainGroupModel>? RemainGroupModels => _remainGroupModels;
        public ArchiveGroupModel? MostProfitabilityArchiveGroup => _archiveGroupModels?.MaxBy(x => x.ArchivesPercent);
        public RemainGroupModel? MostProfitabilityRemainGroup => _remainGroupModels?.MaxBy(x => x.RemainsPercent);
        public long TotalArchiveCount => _totalArchiveCount;
        public double TotalArchiveAmountPurchase => _totalArchiveAmountPurchase;
        public double TotalArchivePercent => _totalArchivePercent;
        public double TotalArchiveAmountSold => _totalArchiveAmountSold;
        public ArchiveElementModel? MostProfitabilityArchive => _mostProfitabilityArchive;
        public long TotalRemainCount => _totalRemainCount;
        public double TotalRemainAmountPurchase => _totalRemainAmountPurchase;
        public double TotalRemainPercent => _totalRemainPercent;
        public double TotalRemainCurrentAmount => _totalRemainCurrentAmount;
        public RemainElementModel? MostProfitabilityRemain => _mostProfitabilityRemain;
        #endregion Properties

        #region Constructor
        public HomeVM()
        {
            var archiveModels = _context?.GetArchiveModels(null);
            _archiveGroupModels = _context?.GetArchiveGroupModels();
            _totalArchiveCount = CalculationModel.GetArchiveTotalCount(archiveModels);
            _totalArchiveAmountPurchase = CalculationModel.GetArchiveTotalAmountPurchase(archiveModels);
            _totalArchivePercent = CalculationModel.GetArchiveAveragePercent(archiveModels);
            _totalArchiveAmountSold = CalculationModel.GetArchiveTotalAmountSold(archiveModels);
            _mostProfitabilityArchive = CalculationModel.GetMostProfitabilityArchive(archiveModels);

            var remainModels = _context?.GetRemainModels(null);
            _remainGroupModels = _context?.GetRemainGroupModels();
            _totalRemainCount = CalculationModel.GetRemainTotalCount(remainModels);
            _totalRemainAmountPurchase = CalculationModel.GetRemainTotalAmountPurchase(remainModels);
            _totalRemainPercent = CalculationModel.GetRemainAveragePercent(remainModels);
            _totalRemainCurrentAmount = CalculationModel.GetRemainTotalCurrentAmount(remainModels);
            _mostProfitabilityRemain = CalculationModel.GetMostProfitabilityRemain(remainModels);
        }
        #endregion Constructor

        #region Methods
        #endregion Methods
    }
}
