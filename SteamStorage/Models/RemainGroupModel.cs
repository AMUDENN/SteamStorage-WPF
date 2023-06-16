using SteamStorage.Entities;

namespace SteamStorage.Models
{
    public class RemainGroupModel
    {
        #region Fields
        private RemainGroup? remainGroup;
        private string title;
        #endregion Fields

        #region Properties
        public RemainGroup? RemainGroup => remainGroup;
        public string Title => title;
        #endregion Properties

        #region Constructor
        public RemainGroupModel(RemainGroup remainGroup)
        {
            this.remainGroup = remainGroup;
            title = remainGroup.Title;
        }
        public RemainGroupModel(string title)
        {
            this.title = title;
        }
        #endregion Constructor

    }
}
