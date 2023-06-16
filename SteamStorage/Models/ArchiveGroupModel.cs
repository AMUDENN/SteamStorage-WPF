using SteamStorage.Entities;
using SteamStorage.Utilities;
using System.Linq;

namespace SteamStorage.Models
{
    public class ArchiveGroupModel
    {
        #region Fields
        private ArchiveGroup archiveGroup;
        private long? archivesCount;
        private double? archivesAmount;
        private double? archivesPercent;
        #endregion Fields

        #region Properties
        public ArchiveGroup ArchiveGroup => archiveGroup;
        public string Title => archiveGroup.Title;
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
        #endregion Constructor

        #region Methods
        private void UpdateRemains()
        {
            var archiveModels = Context.GetArchiveModels(this);
            archivesCount = Context.GetArchiveTotalCount(archiveModels);
            archivesAmount = Context.GetArchiveTotalAmountPurchase(archiveModels);
            archivesPercent = Context.GetArchiveAveragePercent(archiveModels);
        }
        #endregion Methods
    }
}
