using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SteamStorage.Models;
using SteamStorage.Services.Dialog;
using SteamStorage.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SteamStorage.ViewModels
{
    public class RemainSellVM : ObservableObject
    {
        #region Fields
        private RemainElementModel _remainModel;

        private string _countString = string.Empty;
        private string _costSoldString = string.Empty;

        private long _count;
        private double _costSold;
        private ArchiveGroupModel? _selectedArchiveGroupModel;

        private ObservableCollection<ArchiveGroupModel> _groups;

        private RelayCommand _saveCommand;
        private RelayCommand _cancelCommand;

        private readonly Context? _context = Singleton.GetObject<Context>();
        #endregion Fields

        #region Properties
        public string CountString
        {
            get => _countString;
            set
            {
                SetProperty(ref _countString, value.Replace(".", ","));
                SaveCommand.NotifyCanExecuteChanged();
            }
        }
        public string CostSoldString
        {
            get => _costSoldString;
            set
            {
                SetProperty(ref _costSoldString, value.Replace(".", ","));
                SaveCommand.NotifyCanExecuteChanged();
            }
        }
        public long Count
        {
            get => _count;
            set => SetProperty(ref _count, value);
        }
        public double CostSold
        {
            get => _costSold;
            set => SetProperty(ref _costSold, value);
        }
        public ArchiveGroupModel? SelectedArchiveGroupModel
        {
            get => _selectedArchiveGroupModel;
            set
            {
                SetProperty(ref _selectedArchiveGroupModel, value);
                SaveCommand.NotifyCanExecuteChanged();
            }
        }
        public ObservableCollection<ArchiveGroupModel> Groups
        {
            get => _groups;
            set => SetProperty(ref _groups, value);
        }
        #endregion Properties

        #region Commands
        public RelayCommand SaveCommand
        {
            get
            {
                return _saveCommand ??= new RelayCommand(DoSaveCommand, CanExecuteSaveCommand);
            }
        }
        public RelayCommand CancelCommand
        {
            get
            {
                return _cancelCommand ??= new RelayCommand(DoCancelCommand);
            }
        }
        #endregion Commands

        #region Constructor
        public RemainSellVM(RemainElementModel remainModel)
        {
            this._remainModel = remainModel;
            Count = remainModel.Count;
            CountString = remainModel.Count.ToString();
            Groups = new ObservableCollection<ArchiveGroupModel>(_context?.ArchiveGroupModels);
        }
        #endregion Constructor

        #region Methods
        private void DoSaveCommand()
        {
            _remainModel.SellRemain(Count, CostSold, DateTime.Now, SelectedArchiveGroupModel);
            _context?.SaveChanges();
            WindowDialogService.CurrentDialogWindow.DialogResult = true;
        }
        private bool CanExecuteSaveCommand()
        {
            try
            {
                Count = Convert.ToInt64(CountString);
                CostSold = Convert.ToDouble(CostSoldString);
                return true;
            }
            catch
            {
                return false;
            }
        }
        private void DoCancelCommand()
        {
            _context?.UndoChanges();
            WindowDialogService.CurrentDialogWindow.DialogResult = false;
        }
        #endregion Methods
    }
}
