using CommunityToolkit.Mvvm.ComponentModel;
using SteamStorage.Models;
using SteamStorage.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace SteamStorage.ViewModels
{
    public class RemainsVM : ObservableObject
    {
        #region Fields
        private string filter;
        private List<RemainGroupModel> groups;
        private List<RemainModel> remains;
        private List<RemainModel> displayedRemains;
        private RemainGroupModel selectedGroup;
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
        public List<RemainGroupModel> Groups
        {
            get => groups;
            set => SetProperty(ref groups, value);
        }
        public List<RemainModel> Remains
        {
            get => remains;
            set
            {
                SetProperty(ref remains, value);
                DoFiltering();
            }
        }
        public List<RemainModel> DisplayedRemains
        {
            get => displayedRemains;
            set => SetProperty(ref displayedRemains, value);
        }
        public RemainGroupModel SelectedGroup
        {
            get => selectedGroup;
            set
            {
                SetProperty(ref selectedGroup, value);
                DoFiltering();
            }
        }
        #endregion Properties
        public RemainsVM()
        {
            var context = Context.GetContext();

            Groups = context.RemainGroups.Select(x => new RemainGroupModel(x)).ToList();
            Groups.Insert(0, new("Все"));

            SelectedGroup = Groups.First();

            Filter = string.Empty;

            Remains = context.Remains.Select(x => new RemainModel(x)).ToList();
        }
        private void DoFiltering()
        {
            if (Remains is null) return;
            DisplayedRemains = Remains.Where(
                x => (SelectedGroup.RemainGroup is null || x.RemainGroup == SelectedGroup.RemainGroup) && x.Title.ToLower().Contains(Filter)
                ).ToList();
            DoSorting();
        }
        private void DoSorting()
        {

        }
    }
}
