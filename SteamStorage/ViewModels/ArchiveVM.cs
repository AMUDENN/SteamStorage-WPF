using CommunityToolkit.Mvvm.ComponentModel;
using SteamStorage.Models;
using SteamStorage.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace SteamStorage.ViewModels
{
    public class ArchiveVM : ObservableObject
    {
        #region Fields
        private string filter;
        private List<ArchiveGroupModel> groups;
        private List<ArchiveModel> archives;
        private List<ArchiveModel> displayedArchives;
        private ArchiveGroupModel selectedGroup;
        #endregion Fields

        #region Properties
        public string Filter
        {
            get => filter;
            set
            {
                SetProperty(ref filter, value.ToLower());
                DoFiltering();
            }
        }
        public List<ArchiveGroupModel> Groups
        {
            get => groups;
            set => SetProperty(ref groups, value);
        }
        public List<ArchiveModel> Archives
        {
            get => archives;
            set
            {
                SetProperty(ref archives, value);
                DoFiltering();
            }
        }
        public List<ArchiveModel> DisplayedArchives
        {
            get => displayedArchives;
            set => SetProperty(ref displayedArchives, value);
        }
        public ArchiveGroupModel SelectedGroup
        {
            get => selectedGroup;
            set
            {
                SetProperty(ref selectedGroup, value);
                DoFiltering();
            }
        }
        #endregion Properties

        #region Constructor
        public ArchiveVM()
        {
            var context = Context.GetContext();

            Groups = context.ArchiveGroups.Select(x => new ArchiveGroupModel(x)).ToList();
            Groups.Insert(0, new("Все"));

            SelectedGroup = Groups.First();

            Filter = string.Empty;

            Archives = context.Archives.Select(x => new ArchiveModel(x)).ToList();
        }
        #endregion Constructor

        #region Methods
        private void DoFiltering()
        {
            if (Archives is null) return;
            DisplayedArchives = Archives.Where(
                x => (SelectedGroup.ArchiveGroup is null || x.ArchiveGroup == SelectedGroup.ArchiveGroup) && x.Title.ToLower().Contains(Filter)
                ).ToList();
            DoSorting();
        }
        private void DoSorting()
        {

        }
        #endregion Methods
    }
}
