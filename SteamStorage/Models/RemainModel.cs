using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;
using SteamStorage.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;

namespace SteamStorage.Models
{
    public class RemainModel : ObservableObject
    {
        #region Fields
        private ObservableCollection<RemainElementModel> _displayedRemains;
        private ObservableCollection<RemainGroupModel> _groups;

        private string _filter = string.Empty;
        private RemainGroupModel? _selectedGroup;
        private string? _selectedOrderTitle;
        private string? _selectedOrderType;
        private bool _isAllRemainsDisplayed;

        private long _totalCount;
        private double _averageCostPurchase;
        private double _totalAmountPurchase;
        private double _averageCurrentCost;
        private double _averagePercent;
        private double _totalCurrentAmount;

        private bool _isProgressBarVisible;
        private int _progressBarValue;

        private readonly BackgroundWorker _updateInfoWorker = new();
        private readonly Context? _context = Singleton.GetObject<Context>();
        #endregion Fields

        #region Properties
        public ObservableCollection<RemainElementModel> DisplayedRemains
        {
            get => _displayedRemains;
            set
            {
                SetProperty(ref _displayedRemains, value);
                Summarize();
            }
        }
        public ObservableCollection<RemainGroupModel> Groups
        {
            get => _groups;
            set => SetProperty(ref _groups, value);
        }
        public string Filter
        {
            get => _filter;
            set
            {
                SetProperty(ref _filter, value.ToLower());
                Filtering();
            }
        }
        public RemainGroupModel? SelectedGroup
        {
            get => _selectedGroup;
            set
            {
                SetProperty(ref _selectedGroup, value);
                Filtering();
            }
        }
        public readonly Dictionary<string, Func<RemainElementModel, object>> OrderTitles = new()
        {
            { "Название", x => x.Title },
            { "Количество", x => x.Count },
            { "Цена", x => x.CostPurchase },
            { "Сумма", x => x.AmountPurchase },
            { "Дата покупки", x => x.DatePurchase },
            { "Текущая цена", x => x.LastCost },
            { "Изменение", x => x.Percent }
        };
        public string? SelectedOrderTitle
        {
            get => _selectedOrderTitle;
            set
            {
                SetProperty(ref _selectedOrderTitle, value);
                Sorting();
            }
        }
        public readonly Dictionary<string, bool> OrderTypes = new()
        {
            { "По возрастанию", true },
            { "По убыванию", false }
        };
        public string? SelectedOrderType
        {
            get => _selectedOrderType;
            set
            {
                SetProperty(ref _selectedOrderType, value);
                Sorting();
            }
        }
        public long TotalCount
        {
            get => _totalCount;
            set => SetProperty(ref _totalCount, value);
        }
        public double AverageCostPurchase
        {
            get => _averageCostPurchase;
            set => SetProperty(ref _averageCostPurchase, value);
        }
        public double TotalAmountPurchase
        {
            get => _totalAmountPurchase;
            set => SetProperty(ref _totalAmountPurchase, value);
        }
        public double AverageCurrentCost
        {
            get => _averageCurrentCost;
            set => SetProperty(ref _averageCurrentCost, value);
        }
        public double AveragePercent
        {
            get => _averagePercent;
            set => SetProperty(ref _averagePercent, value);
        }
        public double TotalCurrentAmount
        {
            get => _totalCurrentAmount;
            set => SetProperty(ref _totalCurrentAmount, value);
        }
        public bool IsAllRemainsDisplayed
        {
            get => _isAllRemainsDisplayed;
            set
            {
                SetProperty(ref _isAllRemainsDisplayed, value);
                if (_isAllRemainsDisplayed) SelectedGroup = null;
            }
        }
        public bool IsProgressBarVisible
        {
            get => _isProgressBarVisible;
            set => SetProperty(ref _isProgressBarVisible, value);
        }
        public int ProgressBarValue
        {
            get => _progressBarValue;
            set => SetProperty(ref _progressBarValue, value);
        }
        public BackgroundWorker UpdateInfoWorker => _updateInfoWorker;
        #endregion Properties

        #region Constructor
        public RemainModel()
        {
            GetRemainGroups();

            _updateInfoWorker.DoWork += UpdateInfoWork;
            _updateInfoWorker.WorkerSupportsCancellation = false;
            _updateInfoWorker.RunWorkerCompleted += UpdateInfoComplete;
            _updateInfoWorker.WorkerReportsProgress = true;
            _updateInfoWorker.ProgressChanged += UpdateInfoProgress;
        }
        #endregion Constructor

        #region Methods
        public void RemoveFilter()
        {
            Filter = string.Empty;
            SelectedOrderTitle = null;
            SelectedOrderType = null;
            IsAllRemainsDisplayed = true;
        }
        public void UpdateGroup(RemainGroupModel model)
        {
            _updateInfoWorker.RunWorkerAsync(_context?.GetRemainModels(model));
        }
        public void AddGroup()
        {
            var isAdded = UserMessage.AddRemainGroup();
            if (!isAdded) return;
            _context?.UpdateRemainGroupModels();
            GetRemainGroups();
        }
        public void EditGroupCommand(RemainGroupModel model)
        {
            if (IsDefaultGroup(model))
            {
                UserMessage.Error("Эту группу изменить нельзя!");
                return;
            }
            var isEdit = UserMessage.EditRemainGroup(model);
            if (!isEdit) return;
            _context?.UpdateRemainGroupModels();
            GetRemainGroups();
        }
        public void DeleteGroup(RemainGroupModel model)
        {
            if (IsDefaultGroup(model))
            {
                UserMessage.Error("Эту группу удалить нельзя!");
                return;
            }
            var delete = UserMessage.Question($"Вы уверены, что хотите удалить группу: {model.Title}");
            if (!delete) return;
            model.DeleteGroup();
            IsAllRemainsDisplayed = true;
            _context?.UpdateRemainGroupModels();
            _context?.UpdateRemainModels();
            GetRemainGroups();
            Filtering();
        }
        public void DeleteWithSkinsGroup(RemainGroupModel model)
        {
            if (IsDefaultGroup(model))
            {
                UserMessage.Error("Эту группу удалить нельзя!");
                return;
            }
            var delete = UserMessage.Question($"Вы уверены, что хотите удалить группу и находящиеся в ней скины: {model.Title}");
            if (!delete) return;
            model.DeleteGroupWithSkins();
            IsAllRemainsDisplayed = true;
            _context?.UpdateRemainGroupModels();
            _context?.UpdateRemainModels();
            GetRemainGroups();
            Filtering();
        }
        public void UpdateRemain(RemainElementModel model)
        {
            _updateInfoWorker.RunWorkerAsync(new List<RemainElementModel>() { model });
        }
        public void AddRemain()
        {
            var isAdded = UserMessage.AddRemain(SelectedGroup);
            if (!isAdded) return;
            _context?.UpdateRemainModels();
            Filtering();
        }
        public void EditRemain(RemainElementModel model)
        {
            var isEdit = UserMessage.EditRemain(model);
            if (!isEdit) return;
            _context?.UpdateRemainModels();
            Filtering();
        }
        public void SellRemain(RemainElementModel model)
        {
            var isSell = UserMessage.SellRemain(model);
            if (!isSell) return;
            _context?.UpdateRemainModels();
            _context?.UpdateArchiveModels();
            Filtering();
        }
        public void DeleteRemain(RemainElementModel model)
        {
            var delete = UserMessage.Question($"Вы уверены, что хотите удалить элемент: {model.Title}");
            if (!delete) return;
            model.DeleteRemain();
            _context?.UpdateRemainModels();
            Filtering();
        }
        public void Filtering()
        {
            if (SelectedGroup is not null) IsAllRemainsDisplayed = false;

            DisplayedRemains = new ObservableCollection<RemainElementModel>(_context?.GetRemainModels(SelectedGroup).Where(x => x.Title.ToLower().Contains(Filter)));

            Summarize();

            Sorting();
        }
        private void Summarize()
        {
            TotalCount = CalculationModel.GetRemainTotalCount(DisplayedRemains);

            TotalAmountPurchase = CalculationModel.GetRemainTotalAmountPurchase(DisplayedRemains);

            AverageCostPurchase = CalculationModel.GetRemainAverageCostPurchase(DisplayedRemains);

            TotalCurrentAmount = CalculationModel.GetRemainTotalCurrentAmount(DisplayedRemains);

            AverageCurrentCost = CalculationModel.GetRemainAverageCurrentCost(DisplayedRemains);

            AveragePercent = CalculationModel.GetRemainAveragePercent(DisplayedRemains);
        }
        private void Sorting()
        {
            if (DisplayedRemains is null || SelectedOrderType is null || SelectedOrderTitle is null) return;
            DisplayedRemains = new ObservableCollection<RemainElementModel>(OrderTypes[SelectedOrderType] ? DisplayedRemains.OrderBy(OrderTitles[SelectedOrderTitle]) : DisplayedRemains.OrderByDescending(OrderTitles[SelectedOrderTitle]));
        }
        public void GetRemainGroups()
        {
            Groups = new ObservableCollection<RemainGroupModel>(_context?.GetRemainGroupModels());
        }
        public bool IsDefaultGroup(RemainGroupModel remainGroupModel)
        {
            return remainGroupModel.RemainGroup.Id == 1;
        }
        public void UpdateRemainModelsCurrentCosts(IEnumerable<RemainElementModel> remainModels)
        {
            int percentageIncrease = 100 / remainModels.Count();
            int i = 0;
            Random random = new();
            foreach (RemainElementModel remainModel in remainModels)
            {
                remainModel.UpdateCurrentCost();

                //Работает, но это какая-то шляпа ¯\_(ツ)_/¯
                DisplayedRemains = new ObservableCollection<RemainElementModel>(DisplayedRemains.Select(x => x.Remain == remainModel.Remain ? x = remainModel : x));

                _updateInfoWorker.ReportProgress(percentageIncrease);

                i++;
                if (i % 15 == 0) Thread.Sleep(random.Next(23000, 30000));
                Thread.Sleep(random.Next(1500, 2650));
            }
        }
        public void UpdateInfoWork(object? sender, DoWorkEventArgs e)
        {
            IEnumerable<RemainElementModel> arg = (IEnumerable<RemainElementModel>)e.Argument;
            IsProgressBarVisible = true;
            ProgressBarValue = 0;
            UpdateRemainModelsCurrentCosts(arg);
        }
        public void UpdateInfoProgress(object? sender, ProgressChangedEventArgs e)
        {
            ProgressBarValue += e.ProgressPercentage;
        }
        public void UpdateInfoComplete(object? sender, RunWorkerCompletedEventArgs e)
        {
            IsProgressBarVisible = false;
            _context?.UpdateRemainModels();
            Filtering();
        }
        #endregion Methods
    }
}
