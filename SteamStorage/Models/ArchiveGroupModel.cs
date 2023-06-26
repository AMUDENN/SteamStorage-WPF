using SteamStorage.Entities;
using SteamStorage.Utilities;

namespace SteamStorage.Models
{
    public class ArchiveGroupModel
    {
        #region Fields
        private ArchiveGroup archiveGroup;
        private long? archivesCount;
        private double? archivesAmount;
        private double? archivesPercent;

        private Context context = Singleton.GetObject<Context>();
        #endregion Fields

        #region Properties
        public ArchiveGroup ArchiveGroup => archiveGroup;
        public string Title
        {
            get => archiveGroup.Title;
            set => archiveGroup.Title = value;
        }
        public long ArchivesCount
        {
            get
            {
                if (archivesCount is null) UpdateRemains();
                return (long)archivesCount;
            }
        }
        public double ArchivesAmount
        {
            get
            {
                if (archivesAmount is null) UpdateRemains();
                return (double)archivesAmount;
            }
        }
        public double ArchivesPercent
        {
            get
            {
                if (archivesPercent is null) UpdateRemains();
                return (double)archivesPercent;
            }
        }
        #endregion Properties

        #region Constructor
        public ArchiveGroupModel(ArchiveGroup archiveGroup)
        {
            this.archiveGroup = archiveGroup;
        }
        public ArchiveGroupModel() 
        {
            archiveGroup = new();
            context.AddArchiveGroup(archiveGroup);
        }
        #endregion Constructor

        #region Methods
        private void UpdateRemains()
        {
            var archiveModels = context.GetArchiveModels(this);
            archivesCount = CalculationModel.GetArchiveTotalCount(archiveModels);
            archivesAmount = CalculationModel.GetArchiveTotalAmountPurchase(archiveModels);
            archivesPercent = CalculationModel.GetArchiveAveragePercent(archiveModels);
        }
        #endregion Methods
    }
}
