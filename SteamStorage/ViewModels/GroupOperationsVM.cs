using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SteamStorage.Models.EntityModels;
using SteamStorage.Services.Dialog;
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
        private enum CommandType
        {
            Add, Edit
        }
        #endregion Enums

        #region Fields
        private ArchiveGroupElementModel? _archiveGroupModel;
        private RemainGroupElementModel? _remainGroupModel;
        private GroupTypes _groupType;
        private CommandType _commandType;

        private string _title;

        private RelayCommand _saveCommand;
        private RelayCommand _cancelCommand;

        private readonly Context? _context = Singleton.GetService<Context>();
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
        public GroupOperationsVM(ArchiveGroupElementModel archiveGroupModel)
        {
            _archiveGroupModel = archiveGroupModel;
            _groupType = GroupTypes.Archive;
            _commandType = CommandType.Edit;
            Title = archiveGroupModel.Title;
        }
        public GroupOperationsVM(RemainGroupElementModel remainGroupModel)
        {
            _remainGroupModel = remainGroupModel;
            _groupType = GroupTypes.Remain;
            _commandType = CommandType.Edit;
            Title = remainGroupModel.Title;
        }
        public GroupOperationsVM(GroupTypes groupType)
        {
            _commandType = CommandType.Add;
            _groupType = groupType;
            Title = string.Empty;
        }
        #endregion Constructor

        #region Methods
        private void DoSaveCommand()
        {
            WindowDialogService.CurrentDialogWindow.DialogResult = true;
            if (_commandType == CommandType.Add)
            {
                if (_groupType == GroupTypes.Archive) _archiveGroupModel = new(Title);
                else if (_groupType == GroupTypes.Remain) _remainGroupModel = new(Title);
            }
            else
            {
                if (_groupType == GroupTypes.Archive) _archiveGroupModel?.EditGroup(Title);
                else if (_groupType == GroupTypes.Remain) _remainGroupModel?.EditGroup(Title);
            }
            _context?.SaveChanges();
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
