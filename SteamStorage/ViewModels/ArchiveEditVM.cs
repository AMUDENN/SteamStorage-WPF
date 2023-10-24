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
    public class ArchiveEditVM : ObservableObject
    {
        #region Enums
        private enum CommandType
        {
            Add, Edit
        }
        #endregion Enums

        #region Fields
        private ArchiveElementModel _archiveModel;
        private CommandType _selectedCommandType;

        private string _url;
        private string _countString = string.Empty;
        private string _costPurchaseString = string.Empty;
        private string _costSoldString = string.Empty;

        private long _count;
        private double _costPurchase;
        private double _costSold;
        private ArchiveGroupModel? _selectedArchiveGroupModel;

        private ObservableCollection<ArchiveGroupModel> _groups;

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
        public double CostPurchase
        {
            get => _costPurchase;
            set => SetProperty(ref _costPurchase, value);
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
        public ArchiveEditVM(ArchiveElementModel archiveModel)
        {
            this._archiveModel = archiveModel;
            _selectedCommandType = CommandType.Edit;
            Url = archiveModel.Url;
            Count = archiveModel.Count;
            CostPurchase = archiveModel.CostPurchase;
            CostSold = archiveModel.CostSold;
            CountString = archiveModel.Count.ToString();
            CostPurchaseString = archiveModel.CostPurchase.ToString();
            CostSoldString = archiveModel.CostSold.ToString();
            Groups = new ObservableCollection<ArchiveGroupModel>(_context?.ArchiveGroupModels);
            SelectedArchiveGroupModel = Groups.Where(x => x.ArchiveGroup == archiveModel.ArchiveGroup).First();
        }
        public ArchiveEditVM(ArchiveGroupModel? archiveGroupModel)
        {
            _selectedCommandType = CommandType.Add;
            Url = string.Empty;
            Groups = new ObservableCollection<ArchiveGroupModel>(_context?.ArchiveGroupModels);
            SelectedArchiveGroupModel = archiveGroupModel is null ? Groups.First() : Groups.Where(x => x.ArchiveGroup == archiveGroupModel.ArchiveGroup).First();
        }
        #endregion Constructor

        #region Methods
        private void DoSaveCommand()
        {
            if (_selectedCommandType == CommandType.Add)
            {
                _archiveModel = new();
                _archiveModel.EditArchive(Url, Count, CostPurchase, CostSold, DateTime.Now, DateTime.Now, SelectedArchiveGroupModel);
            }
            else _archiveModel.EditArchive(Url, Count, CostPurchase, CostSold, _archiveModel.DatePurchase, _archiveModel.DateSold, SelectedArchiveGroupModel);
            _context?.SaveChanges();
            WindowDialogService.CurrentDialogWindow.DialogResult = true;
        }
        private bool CanExecuteSaveCommand()
        {
            try
            {
                Count = Convert.ToInt64(CountString);
                CostPurchase = Convert.ToDouble(CostPurchaseString);
                CostSold = Convert.ToDouble(CostSoldString);
                return Url.Length >= 30;
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
