using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SteamStorage.Models;
using SteamStorage.Models.EntityModels;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace SteamStorage.ViewModels
{
    public class RemainsVM : ObservableObject
    {
        #region Fields
        private readonly RemainModel _remainModel = new();

        private RelayCommand _removeFilterCommand;
        private RelayCommand<object> _updateGroupCommand;
        private RelayCommand _addGroupCommand;
        private RelayCommand<object> _editGroupCommand;
        private RelayCommand<object> _deleteGroupCommand;
        private RelayCommand<object> _deleteWithSkinsGroupCommand;
        private RelayCommand<object> _updateRemainCommand;
        private RelayCommand _addRemainCommand;
        private RelayCommand<object> _editRemainCommand;
        private RelayCommand<object> _sellRemainCommand;
        private RelayCommand<object> _deleteRemainCommand;
        #endregion Fields

        #region Properties
        public string Filter
        {
            get => _remainModel.Filter;
            set => _remainModel.Filter = value;
        }
        public IEnumerable<string> OrderTitles => _remainModel.OrderTitles.Keys;
        public string? SelectedOrderTitle
        {
            get => _remainModel.SelectedOrderTitle;
            set => _remainModel.SelectedOrderTitle = value;
        }
        public IEnumerable<string> OrderTypes => _remainModel.OrderTypes.Keys;
        public string? SelectedOrderType
        {
            get => _remainModel.SelectedOrderType;
            set => _remainModel.SelectedOrderType = value;
        }
        public ObservableCollection<RemainGroupElementModel> Groups => _remainModel.Groups;
        public ObservableCollection<RemainElementModel> DisplayedRemains => _remainModel.DisplayedRemains;
        public long TotalCount => _remainModel.TotalCount;
        public double AverageCostPurchase => _remainModel.AverageCostPurchase;
        public double TotalAmountPurchase => _remainModel.TotalAmountPurchase;
        public double AverageCurrentCost => _remainModel.AverageCurrentCost;
        public double AveragePercent => _remainModel.AveragePercent;
        public double TotalCurrentAmount => _remainModel.TotalCurrentAmount;
        public RemainGroupElementModel? SelectedGroup
        {
            get => _remainModel.SelectedGroup;
            set => _remainModel.SelectedGroup = value;
        }
        public bool IsAllRemainsDisplayed
        {
            get => _remainModel.IsAllRemainsDisplayed;
            set => _remainModel.IsAllRemainsDisplayed = value;
        }
        public bool IsProgressBarVisible
        {
            get => _remainModel.IsProgressBarVisible;
            set => _remainModel.IsProgressBarVisible = value;
        }
        public int ProgressBarValue
        {
            get => _remainModel.ProgressBarValue;
            set => _remainModel.ProgressBarValue = value;
        }
        public BackgroundWorker UpdateInfoWorker => _remainModel.UpdateInfoWorker;
        #endregion Properties

        #region Commands
        public RelayCommand RemoveFilterCommand
        {
            get
            {
                return _removeFilterCommand ??= new RelayCommand(DoRemoveFilterCommand, CanExecuteRemoveFilterCommand);
            }
        }
        public RelayCommand<object> UpdateGroupCommand
        {
            get
            {
                return _updateGroupCommand ??= new RelayCommand<object>(DoUpdateGroupCommand, CanExecuteUpdateGroupCommand);
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
        public RelayCommand<object> UpdateRemainCommand
        {
            get
            {
                return _updateRemainCommand ??= new RelayCommand<object>(DoUpdateRemainCommand, CanExecuteUpdateRemainCommand);
            }
        }
        public RelayCommand AddRemainCommand
        {
            get
            {
                return _addRemainCommand ??= new RelayCommand(DoAddRemainCommand);
            }
        }
        public RelayCommand<object> EditRemainCommand
        {
            get
            {
                return _editRemainCommand ??= new RelayCommand<object>(DoEditRemainCommand);
            }
        }
        public RelayCommand<object> SellRemainCommand
        {
            get
            {
                return _sellRemainCommand ??= new RelayCommand<object>(DoSellRemainCommand);
            }
        }
        public RelayCommand<object> DeleteRemainCommand
        {
            get
            {
                return _deleteRemainCommand ??= new RelayCommand<object>(DoDeleteRemainCommand);
            }
        }
        #endregion Commands

        #region Constructor
        public RemainsVM()
        {
            _remainModel.PropertyChanged += (s, e) =>
            {
                OnPropertyChanged(e.PropertyName);
                try
                {
                    RemoveFilterCommand.NotifyCanExecuteChanged();
                    UpdateGroupCommand.NotifyCanExecuteChanged();
                }
                catch { } //Надо тут тоже что-то передалать
            };
            IsAllRemainsDisplayed = true;
        }
        #endregion Constructor

        #region Methods
        private void DoRemoveFilterCommand()
        {
            _remainModel.RemoveFilter();
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
            _remainModel.UpdateGroup(data as RemainGroupElementModel);
        }
        private bool CanExecuteUpdateGroupCommand(object? data)
        {
            return !UpdateInfoWorker.IsBusy;
        }
        private void DoAddGroupCommand()
        {
            _remainModel.AddGroup();
        }
        private void DoEditGroupCommand(object? data)
        {
            _remainModel.EditGroupCommand((RemainGroupElementModel)data);
        }
        private void DoDeleteGroupCommand(object? data)
        {
            _remainModel.DeleteGroup((RemainGroupElementModel)data);
        }
        private void DoDeleteWithSkinsGroupCommand(object? data)
        {
            _remainModel.DeleteWithSkinsGroup((RemainGroupElementModel)data);
        }
        private void DoUpdateRemainCommand(object? data)
        {
            _remainModel.UpdateRemain((RemainElementModel)data);
        }
        private bool CanExecuteUpdateRemainCommand(object? data)
        {
            return !UpdateInfoWorker.IsBusy;
        }
        private void DoAddRemainCommand()
        {
            _remainModel.AddRemain();
        }
        private void DoEditRemainCommand(object? data)
        {
            _remainModel.EditRemain((RemainElementModel)data);
        }
        private void DoSellRemainCommand(object? data)
        {
            _remainModel.SellRemain((RemainElementModel)data);
        }
        private void DoDeleteRemainCommand(object? data)
        {
            _remainModel.DeleteRemain((RemainElementModel)data);
        }
        #endregion Methods
    }
}
