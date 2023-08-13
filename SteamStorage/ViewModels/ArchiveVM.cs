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
        private readonly Dictionary<string, Func<ArchiveElementModel, object>> orderTitles = new()
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

        private IEnumerable<ArchiveGroupModel> groups;
        private IEnumerable<ArchiveElementModel> displayedArchives;

        private long totalCount;
        private double averageCostPurchase;
        private double totalAmountPurchase;
        private double averageCostSold;
        private double totalAmountSold;
        private double averagePercent;

        private ArchiveGroupModel? selectedGroup;
        private bool isAllArchivesDisplayed;

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
        public IEnumerable<ArchiveGroupModel> Groups
        {
            get => groups;
            set => SetProperty(ref groups, value);
        }
        public IEnumerable<ArchiveElementModel> DisplayedArchives
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
        public ArchiveGroupModel? SelectedGroup
        {
            get => selectedGroup;
            set
            {
                SetProperty(ref selectedGroup, value);
                DoFiltering();
            }
        }
        public bool IsAllArchivesDisplayed
        {
            get => isAllArchivesDisplayed;
            set
            {
                SetProperty(ref isAllArchivesDisplayed, value);
                if (isAllArchivesDisplayed) SelectedGroup = null;
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
            GetArchiveGroups();
            IsAllArchivesDisplayed = true;
        }
        #endregion Constructor

        #region Methods
        private void DoRemoveFilterCommand()
        {
            Filter = string.Empty;
            SelectedOrderTitle = null;
            SelectedOrderType = null;
            IsAllArchivesDisplayed = true;
        }
        private bool CanExecuteRemoveFilterCommand()
        {
            if (Filter == string.Empty
                && SelectedGroup == null
                && SelectedOrderTitle == null
                && SelectedOrderType == null)
                return false;
            return true;
        }
        private void DoAddGroupCommand()
        {
            var isAdded = UserMessage.AddArchiveGroup();
            if (!isAdded) return;
            Context.UpdateArchiveGroupModels();
            GetArchiveGroups();
        }
        private void DoEditGroupCommand(object? data)
        {
            ArchiveGroupModel model = (ArchiveGroupModel)data;
            if (IsDefaultGroup(model))
            {
                UserMessage.Error("Эту группу изменить нельзя!");
                return;
            }
            var isEdit = UserMessage.EditArchiveGroup(model);
            if (!isEdit) return;
            Context.UpdateArchiveGroupModels();
            GetArchiveGroups();
        }
        private void DoDeleteGroupCommand(object? data)
        {
            ArchiveGroupModel model = (ArchiveGroupModel)data;
            if (IsDefaultGroup(model))
            {
                UserMessage.Error("Эту группу удалить нельзя!");
                return;
            }
            var delete = UserMessage.Question($"Вы уверены, что хотите удалить группу: {model.Title}");
            if (!delete) return;
            model.DeleteGroup();
            Context.UpdateArchiveGroupModels();
            Context.UpdateArchiveModels();
            GetArchiveGroups();
            DoFiltering();
        }
        private void DoDeleteWithSkinsGroupCommand(object? data)
        {
            ArchiveGroupModel model = (ArchiveGroupModel)data;
            if (IsDefaultGroup(model))
            {
                UserMessage.Error("Эту группу удалить нельзя!");
                return;
            }
            var delete = UserMessage.Question($"Вы уверены, что хотите удалить группу и находящиеся в ней скины: {model.Title}");
            if (!delete) return;
            model.DeleteGroupWithSkins();
            Context.UpdateArchiveGroupModels();
            Context.UpdateArchiveModels();
            GetArchiveGroups();
            DoFiltering();
        }
        private void DoAddArchiveCommand()
        {
            var isAdded = UserMessage.AddArchive(SelectedGroup);
            if (!isAdded) return;
            Context.UpdateArchiveModels();
            DoFiltering();
        }
        private void DoEditArchiveCommand(object? data)
        {
            var isEdit = UserMessage.EditArchive((ArchiveElementModel)data);
            if (!isEdit) return;
            Context.UpdateArchiveModels();
            DoFiltering();
        }
        private void DoDeleteArchiveCommand(object? data)
        {
            ArchiveElementModel model = (ArchiveElementModel)data;
            var delete = UserMessage.Question($"Вы уверены, что хотите удалить элемент: {model.Title}");
            if (!delete) return;
            model.DeleteArchive();
            Context.UpdateArchiveModels();
            DoFiltering();
        }
        private void DoFiltering()
        {
            if (SelectedGroup is not null) IsAllArchivesDisplayed = false;

            DisplayedArchives = Context.GetArchiveModels(SelectedGroup).Where(x => x.Title.ToLower().Contains(Filter));

            TotalCount = CalculationModel.GetArchiveTotalCount(DisplayedArchives);

            TotalAmountPurchase = CalculationModel.GetArchiveTotalAmountPurchase(DisplayedArchives);

            AverageCostPurchase = CalculationModel.GetArchiveAverageCostPurchase(DisplayedArchives);

            TotalAmountSold = CalculationModel.GetArchiveTotalAmountSold(DisplayedArchives);

            AverageCostSold = CalculationModel.GetArchiveAverageCostSold(DisplayedArchives);

            AveragePercent = CalculationModel.GetArchiveAveragePercent(DisplayedArchives);

            DoSorting();
        }
        private void DoSorting()
        {
            RemoveFilterCommand.NotifyCanExecuteChanged();
            if (DisplayedArchives is null || SelectedOrderType is null || SelectedOrderTitle is null) return;
            DisplayedArchives = orderTypes[SelectedOrderType] ? DisplayedArchives.OrderBy(orderTitles[SelectedOrderTitle]) : DisplayedArchives.OrderByDescending(orderTitles[SelectedOrderTitle]);
        }
        private void GetArchiveGroups()
        {
            Groups = Context.ArchiveGroups;
        }
        private bool IsDefaultGroup(ArchiveGroupModel archiveGroupModel)
        {
            return archiveGroupModel.ArchiveGroup.Id == 1;
        }
        #endregion Methods
    }
}
