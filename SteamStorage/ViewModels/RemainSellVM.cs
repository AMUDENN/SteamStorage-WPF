using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SteamStorage.Models.EntityModels;
using SteamStorage.Services.Dialog;
using SteamStorage.Utilities;
using System;
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
        private ArchiveGroupElementModel? _selectedArchiveGroupModel;

        private ObservableCollection<ArchiveGroupElementModel> _groups;

        private RelayCommand _saveCommand;
        private RelayCommand _cancelCommand;

        private readonly Context? _context = Singleton.GetService<Context>();
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
        public ArchiveGroupElementModel? SelectedArchiveGroupModel
        {
            get => _selectedArchiveGroupModel;
            set
            {
                SetProperty(ref _selectedArchiveGroupModel, value);
                SaveCommand.NotifyCanExecuteChanged();
            }
        }
        public ObservableCollection<ArchiveGroupElementModel> Groups
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
            Groups = new ObservableCollection<ArchiveGroupElementModel>(_context?.ArchiveGroupModels);
        }
        #endregion Constructor

        #region Methods
        private void DoSaveCommand()
        {
            _remainModel.SellRemain(Count, CostSold, DateTime.Now, SelectedArchiveGroupModel);
            WindowDialogService.CurrentDialogWindow.DialogResult = true;
        }
        private bool CanExecuteSaveCommand()
        {
            try
            {
                Count = Convert.ToInt64(CountString);
                CostSold = Convert.ToDouble(CostSoldString);
                return Count != 0 && CostSold != 0;
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
