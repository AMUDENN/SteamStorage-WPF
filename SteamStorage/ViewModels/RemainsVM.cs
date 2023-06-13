using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SteamStorage.Models;
using SteamStorage.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SteamStorage.ViewModels
{
    public class RemainsVM : ObservableObject
    {
        #region Fields
        private string filter;
        private readonly Dictionary<string, Func<RemainModel, object>> orderTitles = new()
        {
            { "Название", x => x.Title },
            { "Количество", x => x.Count },
            { "Цена", x => x.CostPurchase },
            { "Сумма", x => x.AmountPurchase },
            { "Дата покупки", x => x.DatePurchase },
            { "Текущая цена", x => x.LastCost },
            { "Изменение", x => x.Percent }
        };
        private string? selectedOrderTitle;
        private readonly Dictionary<string, bool> orderTypes = new()
        {
            { "По возрастанию", true },
            { "По убыванию", false }
        };
        private string? selectedOrderType;

        private List<RemainGroupModel> groups;
        private List<RemainModel> remains;
        private List<RemainModel> displayedRemains;

        private double totalCount;
        private double averageCostPurchase;
        private double totalAmountPurchase;
        private double averageCurrentCost;
        private double averagePercent;
        private double totalCurrentAmount;

        private RemainGroupModel selectedGroup;

        private RelayCommand removeFilterCommand;
        private RelayCommand<object> updateGroupCommand;
        private RelayCommand addGroupCommand;
        private RelayCommand<object> editGroupCommand;
        private RelayCommand<object> deleteGroupCommand;
        private RelayCommand<object> deleteWithSkinsGroupCommand;
        private RelayCommand<object> updateRemainCommand;
        private RelayCommand addRemainCommand;
        private RelayCommand<object> editRemainCommand;
        private RelayCommand<object> sellRemainCommand;
        private RelayCommand<object> deleteRemainCommand;
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
        public double TotalCount
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
        public double AverageCurrentCost
        {
            get => averageCurrentCost;
            set => SetProperty(ref averageCurrentCost, value);
        }
        public double AveragePercent
        {
            get => averagePercent;
            set => SetProperty(ref averagePercent, value);
        }
        public double TotalCurrentAmount
        {
            get => totalCurrentAmount;
            set => SetProperty(ref totalCurrentAmount, value);
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

        #region Commands
        public RelayCommand RemoveFilterCommand
        {
            get
            {
                return removeFilterCommand ??= new RelayCommand(DoRemoveFilterCommand, CanExecuteRemoveFilterCommand);
            }
        }
        public RelayCommand<object> UpdateGroupCommand
        {
            get
            {
                return updateGroupCommand ??= new RelayCommand<object>(DoUpdateGroupCommand);
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
        public RelayCommand<object> UpdateRemainCommand
        {
            get
            {
                return updateRemainCommand ??= new RelayCommand<object>(DoUpdateRemainCommand);
            }
        }
        public RelayCommand AddRemainCommand
        {
            get
            {
                return addRemainCommand ??= new RelayCommand(DoAddRemainCommand);
            }
        }
        public RelayCommand<object> EditRemainCommand
        {
            get
            {
                return editRemainCommand ??= new RelayCommand<object>(DoEditRemainCommand);
            }
        }
        public RelayCommand<object> SellRemainCommand
        {
            get
            {
                return sellRemainCommand ??= new RelayCommand<object>(DoSellRemainCommand);
            }
        }
        public RelayCommand<object> DeleteRemainCommand
        {
            get
            {
                return deleteRemainCommand ??= new RelayCommand<object>(DoDeleteRemainCommand);
            }
        }
        #endregion Commands

        #region Constructor
        public RemainsVM()
        {
            var context = Context.GetContext();

            Groups = context.RemainGroups.Select(x => new RemainGroupModel(x)).ToList();
            Groups[1].IsEditable = false;
            Groups.Insert(0, new("Все"));

            SelectedGroup = Groups.First();

            Filter = string.Empty;

            Remains = context.Remains.Select(x => new RemainModel(x)).ToList();
        }
        #endregion Constructor

        #region Methods
        private void DoRemoveFilterCommand()
        {
            Filter = string.Empty;
            SelectedGroup = Groups.First();
            SelectedOrderTitle = null;
            SelectedOrderType = null;
            DisplayedRemains = Remains;
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
        private void DoUpdateGroupCommand(object? data)
        {

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
        private void DoUpdateRemainCommand(object? data)
        {

        }
        private void DoAddRemainCommand()
        {

        }
        private void DoEditRemainCommand(object? data)
        {

        }
        private void DoSellRemainCommand(object? data)
        {

        }
        private void DoDeleteRemainCommand(object? data)
        {

        }
        private void DoFiltering()
        {
            if (Remains is null) return;
            DisplayedRemains = Remains.Where(
                x => (SelectedGroup.RemainGroup is null || x.RemainGroup == SelectedGroup.RemainGroup) && x.Title.ToLower().Contains(Filter)
                ).ToList();

            if (DisplayedRemains.Any())
            {
                TotalCount = DisplayedRemains.Sum(x => x.Count);

                TotalAmountPurchase = DisplayedRemains.Sum(x => x.AmountPurchase);

                AverageCostPurchase = TotalAmountPurchase / TotalCount;

                TotalCurrentAmount = DisplayedRemains.Sum(x => x.LastCost * x.Count);

                AverageCurrentCost = TotalCurrentAmount / TotalCount;

                AveragePercent = (TotalCurrentAmount - TotalAmountPurchase) / TotalAmountPurchase * 100;
            }
            else
            {
                TotalCount = 0;
                AverageCostPurchase = 0;
                TotalAmountPurchase = 0;
                AverageCurrentCost = 0;
                AveragePercent = 0;
                TotalCurrentAmount = 0;
            }

            DoSorting();
        }
        private void DoSorting()
        {
            RemoveFilterCommand.NotifyCanExecuteChanged();
            if (DisplayedRemains is null || SelectedOrderType is null || SelectedOrderTitle is null) return;
            var remains = orderTypes[SelectedOrderType] ? DisplayedRemains.OrderBy(orderTitles[SelectedOrderTitle]) : DisplayedRemains.OrderByDescending(orderTitles[SelectedOrderTitle]);
            DisplayedRemains = remains.ToList();
        }
        #endregion Methods
    }
}
