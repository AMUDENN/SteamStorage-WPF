using CommunityToolkit.Mvvm.ComponentModel;
using SteamStorage.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SteamStorage.Models
{
    public class ArchiveModel : ObservableObject
    {
        #region Fields
        private ObservableCollection<ArchiveElementModel> _displayedArchives;
        private ObservableCollection<ArchiveGroupModel> _groups;

        private string _filter = string.Empty;
        private ArchiveGroupModel? _selectedGroup;
        private string? _selectedOrderTitle;
        private string? _selectedOrderType;
        private bool _isAllArchivesDisplayed;

        private long _totalCount;
        private double _averageCostPurchase;
        private double _totalAmountPurchase;
        private double _averageCostSold;
        private double _totalAmountSold;
        private double _averagePercent;
        #endregion Fields

        #region Properties
        public ObservableCollection<ArchiveElementModel> DisplayedArchives
        {
            get => _displayedArchives;
            set => SetProperty(ref _displayedArchives, value);
        }
        public ObservableCollection<ArchiveGroupModel> Groups
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
        public ArchiveGroupModel? SelectedGroup
        {
            get => _selectedGroup;
            set
            {
                SetProperty(ref _selectedGroup, value);
                Filtering();
            }
        }
        public readonly Dictionary<string, Func<ArchiveElementModel, object>> OrderTitles = new()
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
        public double AverageCostSold
        {
            get => _averageCostSold;
            set => SetProperty(ref _averageCostSold, value);
        }
        public double TotalAmountSold
        {
            get => _totalAmountSold;
            set => SetProperty(ref _totalAmountSold, value);
        }
        public double AveragePercent
        {
            get => _averagePercent;
            set => SetProperty(ref _averagePercent, value);
        }
        public bool IsAllArchivesDisplayed
        {
            get => _isAllArchivesDisplayed;
            set
            {
                SetProperty(ref _isAllArchivesDisplayed, value);
                if (_isAllArchivesDisplayed) SelectedGroup = null;
            }
        }
        #endregion Properties

        #region Constructor
        public ArchiveModel()
        {
            GetArchiveGroups();
        }
        #endregion Constructor

        #region Methods
        public void RemoveFilter()
        {
            Filter = string.Empty;
            SelectedOrderTitle = null;
            SelectedOrderType = null;
            IsAllArchivesDisplayed = true;
        }
        public void AddGroup()
        {
            var isAdded = UserMessage.AddArchiveGroup();
            if (!isAdded) return;
            Context.UpdateArchiveGroupModels();
            GetArchiveGroups();
        }
        public void EditGroup(ArchiveGroupModel model)
        {
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
        public void DeleteGroup(ArchiveGroupModel model)
        {
            if (IsDefaultGroup(model))
            {
                UserMessage.Error("Эту группу удалить нельзя!");
                return;
            }
            var delete = UserMessage.Question($"Вы уверены, что хотите удалить группу: {model.Title}");
            if (!delete) return;
            model.DeleteGroup();
            IsAllArchivesDisplayed = true;
            Context.UpdateArchiveGroupModels();
            Context.UpdateArchiveModels();
            GetArchiveGroups();
            Filtering();
        }
        public void DeleteWithSkinsGroup(ArchiveGroupModel model)
        {
            if (IsDefaultGroup(model))
            {
                UserMessage.Error("Эту группу удалить нельзя!");
                return;
            }
            var delete = UserMessage.Question($"Вы уверены, что хотите удалить группу и находящиеся в ней скины: {model.Title}");
            if (!delete) return;
            model.DeleteGroupWithSkins();
            IsAllArchivesDisplayed = true;
            Context.UpdateArchiveGroupModels();
            Context.UpdateArchiveModels();
            GetArchiveGroups();
            Filtering();
        }
        public void AddArchive()
        {
            var isAdded = UserMessage.AddArchive(SelectedGroup);
            if (!isAdded) return;
            Context.UpdateArchiveModels();
            Filtering();
        }
        public void EditArchive(ArchiveElementModel model)
        {
            var isEdit = UserMessage.EditArchive(model);
            if (!isEdit) return;
            Context.UpdateArchiveModels();
            Filtering();
        }
        public void DeleteArchive(ArchiveElementModel model)
        {
            var delete = UserMessage.Question($"Вы уверены, что хотите удалить элемент: {model.Title}");
            if (!delete) return;
            model.DeleteArchive();
            Context.UpdateArchiveModels();
            Filtering();
        }
        public void Filtering()
        {
            if (SelectedGroup is not null) IsAllArchivesDisplayed = false;

            DisplayedArchives = new ObservableCollection<ArchiveElementModel>(Context.GetArchiveModels(SelectedGroup).Where(x => x.Title.ToLower().Contains(Filter)));

            TotalCount = CalculationModel.GetArchiveTotalCount(DisplayedArchives);

            TotalAmountPurchase = CalculationModel.GetArchiveTotalAmountPurchase(DisplayedArchives);

            AverageCostPurchase = CalculationModel.GetArchiveAverageCostPurchase(DisplayedArchives);

            TotalAmountSold = CalculationModel.GetArchiveTotalAmountSold(DisplayedArchives);

            AverageCostSold = CalculationModel.GetArchiveAverageCostSold(DisplayedArchives);

            AveragePercent = CalculationModel.GetArchiveAveragePercent(DisplayedArchives);

            Sorting();
        }
        public void Sorting()
        {
            if (DisplayedArchives is null || SelectedOrderType is null || SelectedOrderTitle is null) return;
            DisplayedArchives = new ObservableCollection<ArchiveElementModel>(OrderTypes[SelectedOrderType] ? DisplayedArchives.OrderBy(OrderTitles[SelectedOrderTitle]) : DisplayedArchives.OrderByDescending(OrderTitles[SelectedOrderTitle]));
        }
        public void GetArchiveGroups()
        {
            Groups = new ObservableCollection<ArchiveGroupModel>(Context.ArchiveGroups);
        }
        public bool IsDefaultGroup(ArchiveGroupModel archiveGroupModel)
        {
            return archiveGroupModel.ArchiveGroup.Id == 1;
        }
        #endregion Methods
    }
}
