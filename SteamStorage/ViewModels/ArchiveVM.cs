using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SteamStorage.Models;
using SteamStorage.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SteamStorage.ViewModels
{
    public class ArchiveVM : ObservableObject
    {
        #region Fields
        private string filter = string.Empty;
        private readonly Dictionary<string, Func<ArchiveModel, object>> orderTitles = new()
        {
            { "Название", x => x.Title },
            { "Количество", x => x.Count },
            { "Цена покупки", x => x.CostPurchase },
            { "Сумма покупки", x => x.AmountPurchase },
            { "Дата покупки", x => x.DatePurchase },
            { "Цена продажи", x => x.CostSold },
            { "Сумма продажи", x => x.AmountSold },
            { "Дата продажи", x => x.DateSold },
            { "Изменение", x => x.Percent }
        };
        private string? selectedOrderTitle;
        private readonly Dictionary<string, bool> orderTypes = new()
        {
            { "По возрастанию", true },
            { "По убыванию", false }
        };
        private string? selectedOrderType;

        private List<ArchiveGroupModel> groups;
        private List<ArchiveModel> displayedArchives;

        private long totalCount;
        private double averageCostPurchase;
        private double totalAmountPurchase;
        private double averageCostSold;
        private double totalAmountSold;
        private double averagePercent;

        private ArchiveGroupModel selectedGroup;

        private RelayCommand removeFilterCommand;
        private RelayCommand addGroupCommand;
        private RelayCommand<object> editGroupCommand;
        private RelayCommand<object> deleteGroupCommand;
        private RelayCommand<object> deleteWithSkinsGroupCommand;
        private RelayCommand addArchiveCommand;
        private RelayCommand<object> editArchiveCommand;
        private RelayCommand<object> deleteArchiveCommand;
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
        public IEnumerable<string> OrderTitles => orderTitles.Keys;
        public string? SelectedOrderTitle
        {
            get => selectedOrderTitle;
            set
            {
                SetProperty(ref selectedOrderTitle, value);
                DoSorting();
            }
        }
        public IEnumerable<string> OrderTypes => orderTypes.Keys;
        public string? SelectedOrderType
        {
            get => selectedOrderType;
            set
            {
                SetProperty(ref selectedOrderType, value);
                DoSorting();
            }
        }
        public List<ArchiveGroupModel> Groups
        {
            get => groups;
            set => SetProperty(ref groups, value);
        }
        public List<ArchiveModel> DisplayedArchives
        {
            get => displayedArchives;
            set => SetProperty(ref displayedArchives, value);
        }
        public long TotalCount
        {
            get => totalCount;
            set => SetProperty(ref totalCount, value);
        }
        public double AverageCostPurchase
        {
            get => averageCostPurchase;
            set => SetProperty(ref averageCostPurchase, value);
        }
        public double TotalAmountPurchase
        {
            get => totalAmountPurchase;
            set => SetProperty(ref totalAmountPurchase, value);
        }
        public double AverageCostSold
        {
            get => averageCostSold;
            set => SetProperty(ref averageCostSold, value);
        }
        public double TotalAmountSold
        {
            get => totalAmountSold;
            set => SetProperty(ref totalAmountSold, value);
        }
        public double AveragePercent
        {
            get => averagePercent;
            set => SetProperty(ref averagePercent, value);
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

        #region Commands
        public RelayCommand RemoveFilterCommand
        {
            get
            {
                return removeFilterCommand ??= new RelayCommand(DoRemoveFilterCommand, CanExecuteRemoveFilterCommand);
            }
        }
        public RelayCommand AddGroupCommand
        {
            get
            {
                return addGroupCommand ??= new RelayCommand(DoAddGroupCommand);
            }
        }
        public RelayCommand<object> EditGroupCommand
        {
            get
            {
                return editGroupCommand ??= new RelayCommand<object>(DoEditGroupCommand);
            }
        }
        public RelayCommand<object> DeleteGroupCommand
        {
            get
            {
                return deleteGroupCommand ??= new RelayCommand<object>(DoDeleteGroupCommand);
            }
        }
        public RelayCommand<object> DeleteWithSkinsGroupCommand
        {
            get
            {
                return deleteWithSkinsGroupCommand ??= new RelayCommand<object>(DoDeleteWithSkinsGroupCommand);
            }
        }
        public RelayCommand AddArchiveCommand
        {
            get
            {
                return addArchiveCommand ??= new RelayCommand(DoAddArchiveCommand);
            }
        }
        public RelayCommand<object> EditArchiveCommand
        {
            get
            {
                return editArchiveCommand ??= new RelayCommand<object>(DoEditArchiveCommand);
            }
        }
        public RelayCommand<object> DeleteArchiveCommand
        {
            get
            {
                return deleteArchiveCommand ??= new RelayCommand<object>(DoDeleteArchiveCommand);
            }
        }
        #endregion Commands

        #region Constructor
        public ArchiveVM()
        {
            Groups = Context.ArchiveGroups.ToList();
            SelectedGroup = Groups.First();
        }
        #endregion Constructor

        #region Methods
        private void DoRemoveFilterCommand()
        {
            Filter = string.Empty;
            SelectedGroup = Groups.First();
            SelectedOrderTitle = null;
            SelectedOrderType = null;
            DisplayedArchives = Context.GetArchiveModels(null).ToList();
        }
        private bool CanExecuteRemoveFilterCommand()
        {
            if (Filter == string.Empty
                && SelectedGroup == Groups.First()
                && SelectedOrderTitle == null
                && SelectedOrderType == null)
                return false;
            return true;
        }
        private void DoAddGroupCommand()
        {

        }
        private void DoEditGroupCommand(object? data)
        {

        }
        private void DoDeleteGroupCommand(object? data)
        {

        }
        private void DoDeleteWithSkinsGroupCommand(object? data)
        {

        }
        private void DoAddArchiveCommand()
        {

        }
        private void DoEditArchiveCommand(object? data)
        {

        }
        private void DoDeleteArchiveCommand(object? data)
        {

        }
        private void DoFiltering()
        {
            DisplayedArchives = Context.GetArchiveModels(SelectedGroup).Where(x => x.Title.ToLower().Contains(Filter)).ToList();

            TotalCount = Context.GetArchiveTotalCount(DisplayedArchives);

            TotalAmountPurchase = Context.GetArchiveTotalAmountPurchase(DisplayedArchives);

            AverageCostPurchase = Context.GetArchiveAverageCostPurchase(DisplayedArchives);

            TotalAmountSold = Context.GetArchiveTotalAmountSold(DisplayedArchives);

            AverageCostSold = Context.GetArchiveAverageCostSold(DisplayedArchives);

            AveragePercent = Context.GetArchiveAveragePercent(DisplayedArchives);

            DoSorting();
        }
        private void DoSorting()
        {
            RemoveFilterCommand.NotifyCanExecuteChanged();
            if (DisplayedArchives is null || SelectedOrderType is null || SelectedOrderTitle is null) return;
            var remains = orderTypes[SelectedOrderType] ? DisplayedArchives.OrderBy(orderTitles[SelectedOrderTitle]) : DisplayedArchives.OrderByDescending(orderTitles[SelectedOrderTitle]);
            DisplayedArchives = remains.ToList();
        }
        #endregion Methods
    }
}
