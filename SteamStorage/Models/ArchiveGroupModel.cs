using SteamStorage.Entities;

namespace SteamStorage.Models
{
    public class ArchiveGroupModel
    {
        private ArchiveGroup? archiveGroup;
        private string title;
        private bool isEditable;
        public ArchiveGroup? ArchiveGroup => archiveGroup;
        public string Title => title;
        public bool IsEditable => isEditable;
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
    }
}
