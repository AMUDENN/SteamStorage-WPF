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
        public string Title => archiveGroup.Title;
        public long ArchivesCount
        {
            get
            {
                if (archivesCount is null) UpdateArchives();
                return (long)archivesCount;
            }
        }
        public double ArchivesAmount
        {
            get
            {
                if (archivesAmount is null) UpdateArchives();
                return (double)archivesAmount;
            }
        }
        public double ArchivesPercent
        {
            get
            {
                if (archivesPercent is null) UpdateArchives();
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
        private void UpdateArchives()
        {
            var archiveModels = context.GetArchiveModels(this);
            archivesCount = CalculationModel.GetArchiveTotalCount(archiveModels);
            archivesAmount = CalculationModel.GetArchiveTotalAmountPurchase(archiveModels);
            archivesPercent = CalculationModel.GetArchiveAveragePercent(archiveModels);
        }
        public void EditGroup(string title)
        {
            archiveGroup.Title = title;
        }
        public void DeleteGroup() 
        {
        
        }
        public void DeletGroupWithSkins()
        {

        }
        #endregion Methods
    }
}
