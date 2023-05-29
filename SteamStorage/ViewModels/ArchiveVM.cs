using CommunityToolkit.Mvvm.ComponentModel;
using SteamStorage.Models;
using SteamStorage.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace SteamStorage.ViewModels
{
    public class ArchiveVM : ObservableObject
    {
        private string filter;

        private List<ArchiveGroupModel> groups;
        private List<ArchiveModel> archives;
        private List<ArchiveModel> displayedArchives;

        private ArchiveGroupModel selectedGroup;

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
        public ArchiveVM()
        {
            var context = Context.GetContext();

            Archives = context.Archives.Select(x => new ArchiveModel(x)).ToList();

            Groups = context.ArchiveGroups.Select(x => new ArchiveGroupModel(x)).ToList();
            Groups.Insert(0, new("Все"));

            SelectedGroup = Groups.First();
        }
        private void DoFiltering()
        {
            DisplayedArchives = Archives.Where(
                x => (SelectedGroup.ArchiveGroup is null || x.ArchiveGroup == SelectedGroup.ArchiveGroup) && x.Title.ToLower().Contains(Filter)
                ).ToList();
            DoSorting();
        }
        private void DoSorting()
        {

        }
    }
}
