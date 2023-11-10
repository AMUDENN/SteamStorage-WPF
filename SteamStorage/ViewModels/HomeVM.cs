using CommunityToolkit.Mvvm.ComponentModel;
using SteamStorage.Models;
using SteamStorage.Models.EntityModels;
using SteamStorage.Utilities;
using System.Collections.ObjectModel;
using System.Linq;

namespace SteamStorage.ViewModels
{
    public class HomeVM : ObservableObject
    {
        #region Fields
        private ObservableCollection<ArchiveGroupElementModel>? _archiveGroupModels;
        private ObservableCollection<RemainGroupElementModel>? _remainGroupModels;

        private ArchiveGroupElementModel? _mostProfitabilityArchiveGroup;
        private RemainGroupElementModel? _mostProfitabilityRemainGroup;

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

        private readonly Context? _context = Singleton.GetService<Context>();
        #endregion Fields

        #region Properties
        public ObservableCollection<ArchiveGroupElementModel>? ArchiveGroupModels
        {
            get => _archiveGroupModels;
            set => SetProperty(ref _archiveGroupModels, value);
        }
        public ObservableCollection<RemainGroupElementModel>? RemainGroupModels
        {
            get => _remainGroupModels;
            set => SetProperty(ref _remainGroupModels, value);
        }
        public ArchiveGroupElementModel? MostProfitabilityArchiveGroup
        {
            get => _mostProfitabilityArchiveGroup;
            set => SetProperty(ref _mostProfitabilityArchiveGroup, value);
        }
        public RemainGroupElementModel? MostProfitabilityRemainGroup
        {
            get => _mostProfitabilityRemainGroup;
            set => SetProperty(ref _mostProfitabilityRemainGroup, value);
        }
        public long TotalArchiveCount
        {
            get => _totalArchiveCount;
            set => SetProperty(ref _totalArchiveCount, value);
        }
        public double TotalArchiveAmountPurchase
        {
            get => _totalArchiveAmountPurchase;
            set => SetProperty(ref _totalArchiveAmountPurchase, value);
        }
        public double TotalArchivePercent
        {
            get => _totalArchivePercent;
            set => SetProperty(ref _totalArchivePercent, value);
        }
        public double TotalArchiveAmountSold
        {
            get => _totalArchiveAmountSold;
            set => SetProperty(ref _totalArchiveAmountSold, value);
        }
        public ArchiveElementModel? MostProfitabilityArchive
        {
            get => _mostProfitabilityArchive;
            set => SetProperty(ref _mostProfitabilityArchive, value);
        }
        public long TotalRemainCount
        {
            get => _totalRemainCount;
            set => SetProperty(ref _totalRemainCount, value);
        }
        public double TotalRemainAmountPurchase
        {
            get => _totalRemainAmountPurchase;
            set => SetProperty(ref _totalRemainAmountPurchase, value);
        }
        public double TotalRemainPercent
        {
            get => _totalRemainPercent;
            set => SetProperty(ref _totalRemainPercent, value);
        }
        public double TotalRemainCurrentAmount
        {
            get => _totalRemainCurrentAmount;
            set => SetProperty(ref _totalRemainCurrentAmount, value);
        }
        public RemainElementModel? MostProfitabilityRemain
        {
            get => _mostProfitabilityRemain;
            set => SetProperty(ref _mostProfitabilityRemain, value);
        }
        #endregion Properties

        #region Constructor
        public HomeVM()
        {
            UpdateInfo();
            _context.PropertyChanged += (s, e) => UpdateInfo();
        }
        #endregion Constructor

        #region Methods
        private void UpdateInfo()
        {
            var archiveModels = _context?.GetArchiveModels(null);
            ArchiveGroupModels = new ObservableCollection<ArchiveGroupElementModel>(_context?.ArchiveGroupModels);
            MostProfitabilityArchiveGroup = ArchiveGroupModels?.MaxBy(x => x.ArchivesPercent);
            TotalArchiveCount = CalculationModel.GetArchiveTotalCount(archiveModels);
            TotalArchiveAmountPurchase = CalculationModel.GetArchiveTotalAmountPurchase(archiveModels);
            TotalArchivePercent = CalculationModel.GetArchiveAveragePercent(archiveModels);
            TotalArchiveAmountSold = CalculationModel.GetArchiveTotalAmountSold(archiveModels);
            MostProfitabilityArchive = CalculationModel.GetMostProfitabilityArchive(archiveModels);

            var remainModels = _context?.GetRemainModels(null);
            RemainGroupModels = new ObservableCollection<RemainGroupElementModel>(_context?.RemainGroupModels);
            MostProfitabilityRemainGroup = RemainGroupModels?.MaxBy(x => x.RemainsPercent);
            TotalRemainCount = CalculationModel.GetRemainTotalCount(remainModels);
            TotalRemainAmountPurchase = CalculationModel.GetRemainTotalAmountPurchase(remainModels);
            TotalRemainPercent = CalculationModel.GetRemainAveragePercent(remainModels);
            TotalRemainCurrentAmount = CalculationModel.GetRemainTotalCurrentAmount(remainModels);
            MostProfitabilityRemain = CalculationModel.GetMostProfitabilityRemain(remainModels);
        }
        #endregion Methods
    }
}
