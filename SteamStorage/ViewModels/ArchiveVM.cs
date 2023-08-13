using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SteamStorage.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SteamStorage.ViewModels
{
    public class ArchiveVM : ObservableObject
    {
        #region Fields
        private readonly ArchiveModel _archiveModel = new();

        private RelayCommand _removeFilterCommand;
        private RelayCommand _addGroupCommand;
        private RelayCommand<object> _editGroupCommand;
        private RelayCommand<object> _deleteGroupCommand;
        private RelayCommand<object> _deleteWithSkinsGroupCommand;
        private RelayCommand _addArchiveCommand;
        private RelayCommand<object> _editArchiveCommand;
        private RelayCommand<object> _deleteArchiveCommand;
        #endregion Fields

        #region Properties
        public string Filter
        {
            get => _archiveModel.Filter;
            set => _archiveModel.Filter = value;
        }
        public IEnumerable<string> OrderTitles => _archiveModel.OrderTitles.Keys;
        public string? SelectedOrderTitle
        {
            get => _archiveModel.SelectedOrderTitle;
            set => _archiveModel.SelectedOrderTitle = value;
        }
        public IEnumerable<string> OrderTypes => _archiveModel.OrderTypes.Keys;
        public string? SelectedOrderType
        {
            get => _archiveModel.SelectedOrderType;
            set => _archiveModel.SelectedOrderType = value;
        }
        public ObservableCollection<ArchiveGroupModel> Groups => _archiveModel.Groups;
        public ObservableCollection<ArchiveElementModel> DisplayedArchives => _archiveModel.DisplayedArchives;
        public long TotalCount => _archiveModel.TotalCount;
        public double AverageCostPurchase => _archiveModel.AverageCostPurchase;
        public double TotalAmountPurchase => _archiveModel.TotalAmountPurchase;
        public double AverageCostSold => _archiveModel.AverageCostSold;
        public double TotalAmountSold => _archiveModel.TotalAmountSold;
        public double AveragePercent => _archiveModel.AveragePercent;
        public ArchiveGroupModel? SelectedGroup
        {
            get => _archiveModel.SelectedGroup;
            set => _archiveModel.SelectedGroup = value;
        }
        public bool IsAllArchivesDisplayed
        {
            get => _archiveModel.IsAllArchivesDisplayed;
            set => _archiveModel.IsAllArchivesDisplayed = value;
        }
        #endregion Properties

        #region Commands
        public RelayCommand RemoveFilterCommand
        {
            get
            {
                return _removeFilterCommand ??= new RelayCommand(DoRemoveFilterCommand, CanExecuteRemoveFilterCommand);
            }
        }
        public RelayCommand AddGroupCommand
        {
            get
            {
                return _addGroupCommand ??= new RelayCommand(DoAddGroupCommand);
            }
        }
        public RelayCommand<object> EditGroupCommand
        {
            get
            {
                return _editGroupCommand ??= new RelayCommand<object>(DoEditGroupCommand);
            }
        }
        public RelayCommand<object> DeleteGroupCommand
        {
            get
            {
                return _deleteGroupCommand ??= new RelayCommand<object>(DoDeleteGroupCommand);
            }
        }
        public RelayCommand<object> DeleteWithSkinsGroupCommand
        {
            get
            {
                return _deleteWithSkinsGroupCommand ??= new RelayCommand<object>(DoDeleteWithSkinsGroupCommand);
            }
        }
        public RelayCommand AddArchiveCommand
        {
            get
            {
                return _addArchiveCommand ??= new RelayCommand(DoAddArchiveCommand);
            }
        }
        public RelayCommand<object> EditArchiveCommand
        {
            get
            {
                return _editArchiveCommand ??= new RelayCommand<object>(DoEditArchiveCommand);
            }
        }
        public RelayCommand<object> DeleteArchiveCommand
        {
            get
            {
                return _deleteArchiveCommand ??= new RelayCommand<object>(DoDeleteArchiveCommand);
            }
        }
        #endregion Commands

        #region Constructor
        public ArchiveVM()
        {
            _archiveModel.PropertyChanged += (s, e) =>
            {
                OnPropertyChanged(e.PropertyName);
                RemoveFilterCommand.NotifyCanExecuteChanged();
            };
            IsAllArchivesDisplayed = true;
        }
        #endregion Constructor

        #region Methods
        private void DoRemoveFilterCommand()
        {
            _archiveModel.RemoveFilter();
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
            _archiveModel.AddGroup();
        }
        private void DoEditGroupCommand(object? data)
        {
            _archiveModel.EditGroup((ArchiveGroupModel)data);
        }
        private void DoDeleteGroupCommand(object? data)
        {
            _archiveModel.DeleteGroup((ArchiveGroupModel)data);
        }
        private void DoDeleteWithSkinsGroupCommand(object? data)
        {
            _archiveModel.DeleteWithSkinsGroup((ArchiveGroupModel)data);
        }
        private void DoAddArchiveCommand()
        {
            _archiveModel.AddArchive();
        }
        private void DoEditArchiveCommand(object? data)
        {
            _archiveModel.EditArchive((ArchiveElementModel)data);
        }
        private void DoDeleteArchiveCommand(object? data)
        {
            _archiveModel.DeleteArchive((ArchiveElementModel)data);
        }
        #endregion Methods
    }
}
