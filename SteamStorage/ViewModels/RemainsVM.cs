using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SteamStorage.Models;
using SteamStorage.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;

namespace SteamStorage.ViewModels
{
    public class RemainsVM : ObservableObject
    {
        #region Fields
        private string filter = string.Empty;
        private readonly Dictionary<string, Func<RemainElementModel, object>> orderTitles = new()
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
        private IEnumerable<RemainElementModel> displayedRemains;

        private long totalCount;
        private double averageCostPurchase;
        private double totalAmountPurchase;
        private double averageCurrentCost;
        private double averagePercent;
        private double totalCurrentAmount;

        private RemainGroupModel? selectedGroup;
        private bool isAllRemainsDisplayed;

        private bool isProgressBarVisible;
        private int progressBarValue;

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

        private BackgroundWorker updateInfoWorker = new BackgroundWorker();
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
        public IEnumerable<RemainElementModel> DisplayedRemains
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
        public bool IsProgressBarVisible
        {
            get => isProgressBarVisible;
            set => SetProperty(ref isProgressBarVisible, value);
        }
        public int ProgressBarValue
        {
            get => progressBarValue;
            set => SetProperty(ref progressBarValue, value);
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
                return updateGroupCommand ??= new RelayCommand<object>(DoUpdateGroupCommand, CanExecuteUpdateGroupCommand);
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
                return updateRemainCommand ??= new RelayCommand<object>(DoUpdateRemainCommand, CanExecuteUpdateRemainCommand);
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

            updateInfoWorker.DoWork += UpdateInfoWork;
            updateInfoWorker.WorkerSupportsCancellation = false;
            updateInfoWorker.RunWorkerCompleted += UpdateInfoComplete;
            updateInfoWorker.WorkerReportsProgress = true;
            updateInfoWorker.ProgressChanged += UpdateInfoProgress;
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
            updateInfoWorker.RunWorkerAsync(Context.GetRemainModels((RemainGroupModel)data).ToList());
        }
        private bool CanExecuteUpdateGroupCommand(object? data)
        {
            return !updateInfoWorker.IsBusy;
        }
        private void DoAddGroupCommand()
        {
            var isAdded = UserMessage.AddRemainGroup();
            if (!isAdded) return;
            Context.UpdateRemainGroupModels();
            GetRemainGroups();
        }
        private void DoEditGroupCommand(object? data)
        {
            RemainGroupModel model = (RemainGroupModel)data;
            if (IsDefaultGroup(model))
            {
                UserMessage.Error("Эту группу изменить нельзя!");
                return;
            }
            var isEdit = UserMessage.EditRemainGroup(model);
            if (!isEdit) return;
            Context.UpdateRemainGroupModels();
            GetRemainGroups();
        }
        private void DoDeleteGroupCommand(object? data)
        {
            RemainGroupModel model = (RemainGroupModel)data;
            if (IsDefaultGroup(model))
            {
                UserMessage.Error("Эту группу удалить нельзя!");
                return;
            }
            var delete = UserMessage.Question($"Вы уверены, что хотите удалить группу: {model.Title}");
            if (!delete) return;
            model.DeleteGroup();
            Context.UpdateRemainGroupModels();
            Context.UpdateRemainGroupModels();
            GetRemainGroups();
            DoFiltering();
        }
        private void DoDeleteWithSkinsGroupCommand(object? data)
        {
            RemainGroupModel model = (RemainGroupModel)data;
            if (IsDefaultGroup(model))
            {
                UserMessage.Error("Эту группу удалить нельзя!");
                return;
            }
            var delete = UserMessage.Question($"Вы уверены, что хотите удалить группу и находящиеся в ней скины: {model.Title}");
            if (!delete) return;
            model.DeleteGroupWithSkins();
            Context.UpdateRemainGroupModels();
            Context.UpdateRemainModels();
            GetRemainGroups();
            DoFiltering();
        }
        private void DoUpdateRemainCommand(object? data)
        {
            updateInfoWorker.RunWorkerAsync(new List<RemainElementModel>() { (RemainElementModel)data });
        }
        private bool CanExecuteUpdateRemainCommand(object? data)
        {
            return !updateInfoWorker.IsBusy;
        }
        private void DoAddRemainCommand()
        {
            var isAdded = UserMessage.AddRemain(SelectedGroup);
            if (!isAdded) return;
            Context.UpdateRemainModels();
            DoFiltering();
        }
        private void DoEditRemainCommand(object? data)
        {
            var isEdit = UserMessage.EditRemain((RemainElementModel)data);
            if (!isEdit) return;
            Context.UpdateRemainModels();
            DoFiltering();
        }
        private void DoSellRemainCommand(object? data)
        {
            var isSell = UserMessage.SellRemain((RemainElementModel)data);
            if (!isSell) return;
            Context.UpdateRemainModels();
            Context.UpdateArchiveModels();
            DoFiltering();
        }
        private void DoDeleteRemainCommand(object? data)
        {
            RemainElementModel model = (RemainElementModel)data;
            var delete = UserMessage.Question($"Вы уверены, что хотите удалить элемент: {model.Title}");
            if (!delete) return;
            model.DeleteRemain();
            Context.UpdateRemainModels();
            DoFiltering();
        }
        private void DoFiltering()
        {
            if (SelectedGroup is not null) IsAllRemainsDisplayed = false;

            DisplayedRemains = Context.GetRemainModels(SelectedGroup).Where(x => x.Title.ToLower().Contains(Filter));

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
            Groups = Context.RemainGroups;
        }
        private bool IsDefaultGroup(RemainGroupModel remainGroupModel)
        {
            return remainGroupModel.RemainGroup.Id == 1;
        }
        private void UpdateRemainModelsCurrentCosts(IEnumerable<RemainElementModel> remainModels)
        {
            int percentageIncrease = 100 / remainModels.Count();
            foreach (RemainElementModel remainModel in remainModels)
            {
                Thread.Sleep(500);
                remainModel.UpdateCurrentCost();
                updateInfoWorker.ReportProgress(percentageIncrease);
            }
        }
        public void UpdateInfoWork(object sender, DoWorkEventArgs e)
        {
            List<RemainElementModel> arg = (List<RemainElementModel>)e.Argument;
            IsProgressBarVisible = true;
            ProgressBarValue = 0;
            UpdateRemainModelsCurrentCosts(arg);
        }
        public void UpdateInfoProgress(object sender, ProgressChangedEventArgs e)
        {
            ProgressBarValue += e.ProgressPercentage;
        }
        public void UpdateInfoComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            IsProgressBarVisible = false;
            Context.UpdateRemainModels();
            DoFiltering();
        }
        #endregion Methods
    }
}
