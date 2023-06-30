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
        private string filter = string.Empty;
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

        private IEnumerable<RemainGroupModel> groups;
        private IEnumerable<RemainModel> displayedRemains;

        private long totalCount;
        private double averageCostPurchase;
        private double totalAmountPurchase;
        private double averageCurrentCost;
        private double averagePercent;
        private double totalCurrentAmount;

        private RemainGroupModel? selectedGroup;
        private bool isAllRemainsDisplayed;

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

        private Context context = Singleton.GetObject<Context>();
        private UserMessage userMessage = Singleton.GetObject<UserMessage>();
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
        public IEnumerable<RemainGroupModel> Groups
        {
            get => groups;
            set => SetProperty(ref groups, value);
        }
        public IEnumerable<RemainModel> DisplayedRemains
        {
            get => displayedRemains;
            set => SetProperty(ref displayedRemains, value);
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
        public RemainGroupModel? SelectedGroup
        {
            get => selectedGroup;
            set
            {
                SetProperty(ref selectedGroup, value);
                DoFiltering();
            }
        }
        public bool IsAllRemainsDisplayed
        {
            get => isAllRemainsDisplayed;
            set
            {
                SetProperty(ref isAllRemainsDisplayed, value);
                if (isAllRemainsDisplayed) SelectedGroup = null;
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
            GetRemainGroups();
            IsAllRemainsDisplayed = true;
        }
        #endregion Constructor

        #region Methods
        private void DoRemoveFilterCommand()
        {
            Filter = string.Empty;
            SelectedOrderTitle = null;
            SelectedOrderType = null;
            IsAllRemainsDisplayed = true;
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
        private void DoUpdateGroupCommand(object? data)
        {

        }
        private void DoAddGroupCommand()
        {
            var isAdded = userMessage.AddRemainGroup();
            if (!isAdded) return;
            context.UpdateRemainGroupModels();
            GetRemainGroups();
        }
        private void DoEditGroupCommand(object? data)
        {
            var isEdit = userMessage.EditRemainGroup((RemainGroupModel)data);
            if (!isEdit) return;
            context.UpdateRemainGroupModels();
            GetRemainGroups();
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
            var isAdded = userMessage.AddRemain(SelectedGroup);
            if (!isAdded) return;
            context.UpdateRemainModels();
            DoFiltering();
        }
        private void DoEditRemainCommand(object? data)
        {
            var isEdit = userMessage.EditRemain((RemainModel)data);
            if (!isEdit) return;
            context.UpdateRemainModels();
            DoFiltering();
        }
        private void DoSellRemainCommand(object? data)
        {
            var isSell = userMessage.SellRemain((RemainModel)data);
            if (!isSell) return;
            context.UpdateRemainModels();
            DoFiltering();
        }
        private void DoDeleteRemainCommand(object? data)
        {
            RemainModel model = (RemainModel)data;
            var delete = userMessage.Question($"Вы уверены, что хотите удалить элемент: {model.Title}");
            if (!delete) return;
            model.DeleteRemain();
            context.UpdateRemainModels();
            DoFiltering();
        }
        private void DoFiltering()
        {
            if (SelectedGroup is not null) IsAllRemainsDisplayed = false;

            DisplayedRemains = context.GetRemainModels(SelectedGroup).Where(x => x.Title.ToLower().Contains(Filter));

            TotalCount = CalculationModel.GetRemainTotalCount(DisplayedRemains);

            TotalAmountPurchase = CalculationModel.GetRemainTotalAmountPurchase(DisplayedRemains);

            AverageCostPurchase = CalculationModel.GetRemainAverageCostPurchase(DisplayedRemains);

            TotalCurrentAmount = CalculationModel.GetRemainTotalCurrentAmount(DisplayedRemains);

            AverageCurrentCost = CalculationModel.GetRemainAverageCurrentCost(DisplayedRemains);

            AveragePercent = CalculationModel.GetRemainAveragePercent(DisplayedRemains);

            DoSorting();
        }
        private void DoSorting()
        {
            RemoveFilterCommand.NotifyCanExecuteChanged();
            if (DisplayedRemains is null || SelectedOrderType is null || SelectedOrderTitle is null) return;
            DisplayedRemains = orderTypes[SelectedOrderType] ? DisplayedRemains.OrderBy(orderTitles[SelectedOrderTitle]) : DisplayedRemains.OrderByDescending(orderTitles[SelectedOrderTitle]);
        }
        private void GetRemainGroups()
        {
            Groups = context.RemainGroups.ToList();
        }
        #endregion Methods
    }
}
