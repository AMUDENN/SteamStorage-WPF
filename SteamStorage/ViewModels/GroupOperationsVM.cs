using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SteamStorage.Models;
using SteamStorage.Services;
using SteamStorage.Utilities;

namespace SteamStorage.ViewModels
{
    public class GroupOperationsVM : ObservableObject
    {
        #region Enums
        public enum GroupTypes
        {
            Archive, Remain
        }
        #endregion Enums

        #region Fields
        private ArchiveGroupModel? _archiveGroupModel;
        private RemainGroupModel? _remainGroupModel;
        private GroupTypes _groupType;

        private string _title;

        private RelayCommand _saveCommand;
        private RelayCommand _cancelCommand;

        private readonly Context? _context = Singleton.GetObject<Context>();
        #endregion Fields

        #region Properties
        public string Title
        {
            get => _title;
            set
            {
                SetProperty(ref _title, value);
                SaveCommand.NotifyCanExecuteChanged();
            }
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

        public GroupOperationsVM(ArchiveGroupModel archiveGroupModel)
        {
            this._archiveGroupModel = archiveGroupModel;
            this._groupType = GroupTypes.Archive;
            Title = archiveGroupModel.Title;
        }
        public GroupOperationsVM(RemainGroupModel remainGroupModel)
        {
            this._remainGroupModel = remainGroupModel;
            this._groupType = GroupTypes.Remain;
            Title = remainGroupModel.Title;
        }
        public GroupOperationsVM(GroupTypes groupType)
        {
            if (groupType == GroupTypes.Archive) _archiveGroupModel = new();
            else if (groupType == GroupTypes.Remain) _remainGroupModel = new();
            this._groupType = groupType;
            Title = string.Empty;
        }
        #endregion Constructor

        #region Methods
        private void DoSaveCommand()
        {
            if (_groupType == GroupTypes.Archive) _archiveGroupModel?.EditGroup(Title);
            else if (_groupType == GroupTypes.Remain) _remainGroupModel?.EditGroup(Title);
            _context?.SaveChanges();
            WindowDialogService.CurrentDialogWindow.DialogResult = true;
        }
        private bool CanExecuteSaveCommand()
        {
            return Title.Length >= 3;
        }
        private void DoCancelCommand()
        {
            _context?.UndoChanges();
            WindowDialogService.CurrentDialogWindow.DialogResult = false;
        }
        #endregion Methods
    }
}
