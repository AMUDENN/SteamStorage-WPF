using CommunityToolkit.Mvvm.ComponentModel;
using SteamStorage.Models.EntityModels;
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
        private ObservableCollection<ArchiveElementModel> _archiveElementModels;
        private ObservableCollection<ArchiveElementModel> _displayedArchives;
        private ObservableCollection<ArchiveGroupElementModel> _groups;

        private string _filter = string.Empty;
        private ArchiveGroupElementModel? _selectedGroup;
        private string? _selectedOrderTitle;
        private string? _selectedOrderType;
        private bool _isAllArchivesDisplayed;

        private long _totalCount;
        private double _averageCostPurchase;
        private double _totalAmountPurchase;
        private double _averageCostSold;
        private double _totalAmountSold;
        private double _averagePercent;

        private readonly Context? _context = Singleton.GetService<Context>();
        #endregion Fields

        #region Properties
        private ObservableCollection<ArchiveElementModel> ArchiveElementModels
        {
            get => _archiveElementModels;
            set
            {
                SetProperty(ref _archiveElementModels, value);
                Filtering();
            }
        }
        public ObservableCollection<ArchiveElementModel> DisplayedArchives
        {
            get => _displayedArchives;
            set
            {
                SetProperty(ref _displayedArchives, value);
                Summarize();
            }
        }
        public ObservableCollection<ArchiveGroupElementModel> Groups
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
        public ArchiveGroupElementModel? SelectedGroup
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
            GetArchiveElements();
            _context.PropertyChanged += (s, e) =>
            {
                switch (e.PropertyName)
                {
                    case "ArchiveElementModels":
                        GetArchiveElements();
                        break;
                    case "ArchiveGroupModels":
                        GetArchiveGroups();
                        break;
                }
            };
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
            UserMessage.AddArchiveGroup();
        }
        public void EditGroup(ArchiveGroupElementModel model)
        {
            if (IsDefaultGroup(model))
            {
                UserMessage.Error("Эту группу изменить нельзя!");
                return;
            }
            UserMessage.EditArchiveGroup(model);
        }
        public void DeleteGroup(ArchiveGroupElementModel model)
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
        }
        public void DeleteWithSkinsGroup(ArchiveGroupElementModel model)
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
        }
        public void AddArchive()
        {
            UserMessage.AddArchive(SelectedGroup);
        }
        public void EditArchive(ArchiveElementModel model)
        {
            UserMessage.EditArchive(model);
        }
        public void DeleteArchive(ArchiveElementModel model)
        {
            var delete = UserMessage.Question($"Вы уверены, что хотите удалить элемент: {model.Title}");
            if (!delete) return;
            model.DeleteArchive();
        }
        private void Filtering()
        {
            if (SelectedGroup is not null) IsAllArchivesDisplayed = false;

            DisplayedArchives = new ObservableCollection<ArchiveElementModel>(ArchiveElementModels.Where(x => SelectedGroup is null || x.ArchiveGroup == SelectedGroup.ArchiveGroup).Where(x => x.Title.ToLower().Contains(Filter)));

            Summarize();

            Sorting();
        }
        private void Summarize()
        {
            TotalCount = CalculationModel.GetArchiveTotalCount(DisplayedArchives);

            TotalAmountPurchase = CalculationModel.GetArchiveTotalAmountPurchase(DisplayedArchives);

            AverageCostPurchase = CalculationModel.GetArchiveAverageCostPurchase(DisplayedArchives);

            TotalAmountSold = CalculationModel.GetArchiveTotalAmountSold(DisplayedArchives);

            AverageCostSold = CalculationModel.GetArchiveAverageCostSold(DisplayedArchives);

            AveragePercent = CalculationModel.GetArchiveAveragePercent(DisplayedArchives);
        }
        private void Sorting()
        {
            if (DisplayedArchives is null || SelectedOrderType is null || SelectedOrderTitle is null) return;
            DisplayedArchives = new ObservableCollection<ArchiveElementModel>(OrderTypes[SelectedOrderType] ? DisplayedArchives.OrderBy(OrderTitles[SelectedOrderTitle]) : DisplayedArchives.OrderByDescending(OrderTitles[SelectedOrderTitle]));
        }
        private void GetArchiveGroups()
        {
            Groups = new ObservableCollection<ArchiveGroupElementModel>(_context?.ArchiveGroupModels);
        }
        private void GetArchiveElements()
        {
            ArchiveElementModels = new ObservableCollection<ArchiveElementModel>(_context?.ArchiveElementModels);
        }
        private bool IsDefaultGroup(ArchiveGroupElementModel archiveGroupModel)
        {
            return archiveGroupModel.ArchiveGroup.Id == 1;
        }
        #endregion Methods
    }
}
