using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SteamStorage.Models;
using SteamStorage.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;

namespace SteamStorage.ViewModels
{
    public class RemainsVM : ObservableObject
    {
        #region Fields
        private readonly RemainModel _remainModel = new();

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
        public ObservableCollection<RemainGroupModel> Groups => _remainModel.Groups;
        public ObservableCollection<RemainElementModel> DisplayedRemains => _remainModel.DisplayedRemains;
        public long TotalCount => _remainModel.TotalCount;
        public double AverageCostPurchase => _remainModel.AverageCostPurchase;
        public double TotalAmountPurchase => _remainModel.TotalAmountPurchase;
        public double AverageCurrentCost => _remainModel.AverageCurrentCost;
        public double AveragePercent => _remainModel.AveragePercent;
        public double TotalCurrentAmount => _remainModel.TotalCurrentAmount;
        public RemainGroupModel? SelectedGroup
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
            _remainModel.PropertyChanged += (s, e) =>
            {
                OnPropertyChanged(e.PropertyName);
                //RemoveFilterCommand.NotifyCanExecuteChanged(); Что-то там с потоками не работает :(
                //UpdateGroupCommand.NotifyCanExecuteChanged();
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
            _remainModel.UpdateGroup((RemainGroupModel)data);
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
            _remainModel.EditGroupCommand((RemainGroupModel)data);
        }
        private void DoDeleteGroupCommand(object? data)
        {
           _remainModel.DeleteGroup((RemainGroupModel)data);
        }
        private void DoDeleteWithSkinsGroupCommand(object? data)
        {
            _remainModel.DeleteWithSkinsGroup((RemainGroupModel)data);
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
