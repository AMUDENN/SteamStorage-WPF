using SteamStorage.Entities;

namespace SteamStorage.Models
{
    public class RemainGroupModel
    {
        #region Fields
        private RemainGroup? remainGroup;
        private string title;
        private bool isEditable;
        #endregion Fields

        #region Properties
        public RemainGroup? RemainGroup => remainGroup;
        public string Title => title;
        public bool IsEditable => isEditable;
        #endregion Properties

        #region Constructor
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
        #endregion Constructor
    }
}
