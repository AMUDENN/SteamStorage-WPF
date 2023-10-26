using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SteamStorage.Models;
using SteamStorage.Services.Dialog;
using SteamStorage.Utilities;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace SteamStorage.ViewModels
{
    public class RemainEditVM : ObservableObject
    {
        #region Enums
        private enum CommandType
        {
            Add, Edit
        }
        #endregion Enums

        #region Fields
        private RemainElementModel _remainModel;
        private CommandType _selectedCommandType;

        private string _url;
        private string _countString = string.Empty;
        private string _costPurchaseString = string.Empty;

        private long _count;
        private double _costPurchase;
        private RemainGroupModel? _selectedRemainGroupModel;

        private ObservableCollection<RemainGroupModel> _groups;

        private RelayCommand _saveCommand;
        private RelayCommand _cancelCommand;

        private readonly Context? _context = Singleton.GetObject<Context>();
        #endregion Fields

        #region Properties
        public string Url
        {
            get => _url;
            set
            {
                SetProperty(ref _url, value);
                SaveCommand.NotifyCanExecuteChanged();
            }
        }
        public string CountString
        {
            get => _countString;
            set
            {
                SetProperty(ref _countString, value.Replace(".", ","));
                SaveCommand.NotifyCanExecuteChanged();
            }
        }
        public string CostPurchaseString
        {
            get => _costPurchaseString;
            set
            {
                SetProperty(ref _costPurchaseString, value.Replace(".", ","));
                SaveCommand.NotifyCanExecuteChanged();
            }
        }
        public long Count
        {
            get => _count;
            set => SetProperty(ref _count, value);
        }
        public double CostPurchase
        {
            get => _costPurchase;
            set => SetProperty(ref _costPurchase, value);
        }
        public RemainGroupModel? SelectedRemainGroupModel
        {
            get => _selectedRemainGroupModel;
            set
            {
                SetProperty(ref _selectedRemainGroupModel, value);
                SaveCommand.NotifyCanExecuteChanged();
            }
        }
        public ObservableCollection<RemainGroupModel> Groups
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
        public RemainEditVM(RemainElementModel remainModel)
        {
            this._remainModel = remainModel;
            _selectedCommandType = CommandType.Edit;
            Url = remainModel.Url;
            Count = remainModel.Count;
            CostPurchase = remainModel.CostPurchase;
            CountString = remainModel.Count.ToString();
            CostPurchaseString = remainModel.CostPurchase.ToString();
            Groups = new ObservableCollection<RemainGroupModel>(_context?.RemainGroupModels);
            SelectedRemainGroupModel = Groups.Where(x => x.RemainGroup == remainModel.RemainGroup).First();
        }
        public RemainEditVM(RemainGroupModel? remainGroupModel)
        {
            
            _selectedCommandType = CommandType.Add;
            Url = string.Empty;
            Groups = new ObservableCollection<RemainGroupModel>(_context?.RemainGroupModels);
            SelectedRemainGroupModel = remainGroupModel is null ? Groups.First() : Groups.Where(x => x.RemainGroup == remainGroupModel.RemainGroup).First();
        }
        #endregion Constructor

        #region Methods
        private void DoSaveCommand()
        {
            if (_selectedCommandType == CommandType.Add)
            {
                _remainModel = new(Url, Count, CostPurchase, DateTime.Now, SelectedRemainGroupModel);
            }
            else _remainModel.EditRemain(Url, Count, CostPurchase, _remainModel.DatePurchase, SelectedRemainGroupModel);
            WindowDialogService.CurrentDialogWindow.DialogResult = true;
        }
        private bool CanExecuteSaveCommand()
        {
            try
            {
                Count = Convert.ToInt64(CountString);
                CostPurchase = Convert.ToDouble(CostPurchaseString);
                return Url.Length >= 30;
            }
            catch
            {
                return false;
            }
        }
        private void DoCancelCommand()
        {
            WindowDialogService.CurrentDialogWindow.DialogResult = false;
        }
        #endregion Methods
    }
}
