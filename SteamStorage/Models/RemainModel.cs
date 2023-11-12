using CommunityToolkit.Mvvm.ComponentModel;
using SteamStorage.Models.EntityModels;
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
        private ObservableCollection<RemainElementModel> _remainElementModels;
        private ObservableCollection<RemainElementModel> _displayedRemains;
        private ObservableCollection<RemainGroupElementModel> _groups;

        private string _filter = string.Empty;
        private RemainGroupElementModel? _selectedGroup;
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
        private readonly Context? _context = Singleton.GetService<Context>();
        #endregion Fields

        #region Properties
        private ObservableCollection<RemainElementModel> RemainElementModels
        {
            get => _remainElementModels;
            set 
            {
                SetProperty(ref _remainElementModels, value);
                Filtering();
            }
        }
        public ObservableCollection<RemainElementModel> DisplayedRemains
        {
            get => _displayedRemains;
            set
            {
                SetProperty(ref _displayedRemains, value);
                Summarize();
            }
        }
        public ObservableCollection<RemainGroupElementModel> Groups
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
        public RemainGroupElementModel? SelectedGroup
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
            GetRemainElements();
            _context.PropertyChanged += (s, e) =>
            {
                switch (e.PropertyName)
                {
                    case "RemainElementModels":
                        GetRemainElements();
                        break;
                    case "RemainGroupModels":
                        GetRemainGroups();
                        break;
                }
            };

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
        public void UpdateGroup(RemainGroupElementModel? model)
        {
            var remainElementModels = RemainElementModels.Where(x => model is null || x.RemainGroup == model.RemainGroup).ToList();
            if (remainElementModels.Any())
                _updateInfoWorker.RunWorkerAsync(remainElementModels);
        }
        public void AddGroup()
        {
            UserMessage.AddRemainGroup();
        }
        public void EditGroupCommand(RemainGroupElementModel model)
        {
            if (IsDefaultGroup(model))
            {
                UserMessage.Error("Эту группу изменить нельзя!");
                return;
            }
            UserMessage.EditRemainGroup(model);
        }
        public void DeleteGroup(RemainGroupElementModel model)
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
        }
        public void DeleteWithSkinsGroup(RemainGroupElementModel model)
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
        }
        public void UpdateRemain(RemainElementModel model)
        {
            _updateInfoWorker.RunWorkerAsync(new List<RemainElementModel>() { model });
        }
        public void AddRemain()
        {
            UserMessage.AddRemain(SelectedGroup);
        }
        public void EditRemain(RemainElementModel model)
        {
            UserMessage.EditRemain(model);
        }
        public void SellRemain(RemainElementModel model)
        {
            UserMessage.SellRemain(model);
        }
        public void DeleteRemain(RemainElementModel model)
        {
            var delete = UserMessage.Question($"Вы уверены, что хотите удалить элемент: {model.Title}");
            if (!delete) return;
            model.DeleteRemain();
        }
        public void Filtering()
        {
            if (SelectedGroup is not null) IsAllRemainsDisplayed = false;

            DisplayedRemains = new ObservableCollection<RemainElementModel>(RemainElementModels.Where(x => SelectedGroup is null || x.RemainGroup == SelectedGroup.RemainGroup).Where(x => x.Title.ToLower().Contains(Filter)));

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
        private void GetRemainGroups()
        {
            Groups = new ObservableCollection<RemainGroupElementModel>(_context?.RemainGroupModels);
        }
        private void GetRemainElements()
        {
            RemainElementModels = new ObservableCollection<RemainElementModel>(_context?.RemainElementModels);
        }
        private bool IsDefaultGroup(RemainGroupElementModel remainGroupModel)
        {
            return remainGroupModel.RemainGroup.Id == 1;
        }
        private void UpdateRemainModelsCurrentCosts(IEnumerable<RemainElementModel> remainModels)
        {
            int percentageIncrease = 100 / remainModels.Count();
            int i = 0;
            Random random = new();
            foreach (RemainElementModel remainModel in remainModels)
            {
                remainModel.UpdateCurrentCost();

                Filtering();

                _updateInfoWorker.ReportProgress(percentageIncrease);

                i++;
                if (i % 15 == 0) Thread.Sleep(random.Next(23000, 30000));
                Thread.Sleep(random.Next(1500, 2650));
            }
        }
        private void UpdateInfoWork(object? sender, DoWorkEventArgs e)
        {
            IEnumerable<RemainElementModel> arg = (IEnumerable<RemainElementModel>)e.Argument;
            IsProgressBarVisible = true;
            ProgressBarValue = 0;
            UpdateRemainModelsCurrentCosts(arg);
        }
        private void UpdateInfoProgress(object? sender, ProgressChangedEventArgs e)
        {
            ProgressBarValue += e.ProgressPercentage;
        }
        private void UpdateInfoComplete(object? sender, RunWorkerCompletedEventArgs e)
        {
            IsProgressBarVisible = false;
            _context?.UpdateRemainModels();
        }
        #endregion Methods
    }
}
