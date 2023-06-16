using SteamStorage.Entities;

namespace SteamStorage.Models
{
    public class ArchiveGroupModel
    {
        #region Fields
        private ArchiveGroup? archiveGroup;
        private string title;
        #endregion Fields

        #region Properties
        public ArchiveGroup? ArchiveGroup => archiveGroup;
        public string Title => title;
        #endregion Properties

        #region Constructor
        public ArchiveGroupModel(ArchiveGroup archiveGroup)
        {
            this.archiveGroup = archiveGroup;
            title = archiveGroup.Title;
        }
        public ArchiveGroupModel(string title)
        {
            this.title = title;
        }
        #endregion Constructor
    }
}
