using SteamStorage.Entities;

namespace SteamStorage.Models
{
    public class RemainGroupModel
    {
        private RemainGroup? remainGroup;
        private string title;
        private bool isEditable;
        public RemainGroup? RemainGroup => remainGroup;
        public string Title => title;
        public bool IsEditable => isEditable;
        public RemainGroupModel(RemainGroup remainGroup)
        {
            this.remainGroup = remainGroup;
            title = remainGroup.Title;
            isEditable = true;
        }
        public RemainGroupModel(string title)
        {
            this.title = title;
            isEditable = false;
        }
    }
}
