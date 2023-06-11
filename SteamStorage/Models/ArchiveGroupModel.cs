using SteamStorage.Entities;

namespace SteamStorage.Models
{
    public class ArchiveGroupModel
    {
        #region Fields
        private ArchiveGroup? archiveGroup;
        private string title;
        private bool isEditable;
        #endregion Fields

        #region Properties
        public ArchiveGroup? ArchiveGroup => archiveGroup;
        public string Title => title;
        public bool IsEditable => isEditable;
        #endregion Properties

        #region Constructor
        public ArchiveGroupModel(ArchiveGroup archiveGroup)
        {
            this.archiveGroup = archiveGroup;
            title = archiveGroup.Title;
            isEditable = true;
        }
        public ArchiveGroupModel(string title)
        {
            this.title = title;
            isEditable = false;
        }
        #endregion Constructor
    }
}
