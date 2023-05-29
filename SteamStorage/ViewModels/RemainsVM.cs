using CommunityToolkit.Mvvm.ComponentModel;
using SteamStorage.Models;
using SteamStorage.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace SteamStorage.ViewModels
{
    public class RemainsVM : ObservableObject
    {
        private string filter;

        private List<RemainGroupModel> groups;
        private List<RemainModel> remains;
        private List<RemainModel> displayedRemains;

        private RemainGroupModel selectedGroup;

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
        public RemainsVM()
        {
            var context = Context.GetContext();

            Remains = context.Remains.Select(x => new RemainModel(x)).ToList();

            Groups = context.RemainGroups.Select(x => new RemainGroupModel(x)).ToList();
            Groups.Insert(0, new("Все"));

            SelectedGroup = Groups.First();
        }
        private void DoFiltering()
        {
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
