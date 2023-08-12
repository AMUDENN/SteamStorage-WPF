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
        private ArchiveGroupModel? archiveGroupModel;
        private RemainGroupModel? remainGroupModel;
        private GroupTypes groupType;

        private string title;

        private RelayCommand saveCommand;
        private RelayCommand cancelCommand;
        #endregion Fields

        #region Properties
        public string Title
        {
            get => title;
            set
            {
                SetProperty(ref title, value);
                SaveCommand.NotifyCanExecuteChanged();
            }
        }
        #endregion Properties

        #region Commands
        public RelayCommand SaveCommand
        {
            get
            {
                return saveCommand ??= new RelayCommand(DoSaveCommand, CanExecuteSaveCommand);
            }
        }
        public RelayCommand CancelCommand
        {
            get
            {
                return cancelCommand ??= new RelayCommand(DoCancelCommand);
            }
        }
        #endregion Commands

        #region Constructor

        public GroupOperationsVM(ArchiveGroupModel archiveGroupModel)
        {
            this.archiveGroupModel = archiveGroupModel;
            this.groupType = GroupTypes.Archive;
            Title = archiveGroupModel.Title;
        }
        public GroupOperationsVM(RemainGroupModel remainGroupModel)
        {
            this.remainGroupModel = remainGroupModel;
            this.groupType = GroupTypes.Remain;
            Title = remainGroupModel.Title;
        }
        public GroupOperationsVM(GroupTypes groupType)
        {
            if (groupType == GroupTypes.Archive) archiveGroupModel = new();
            else if (groupType == GroupTypes.Remain) remainGroupModel = new();
            this.groupType = groupType;
            Title = string.Empty;
        }
        #endregion Constructor

        #region Methods
        private void DoSaveCommand()
        {
            if (groupType == GroupTypes.Archive) archiveGroupModel.EditGroup(Title);
            else if (groupType == GroupTypes.Remain) remainGroupModel.EditGroup(Title);
            Context.SaveChanges();
            WindowDialogService.CurrentDialogWindow.DialogResult = true;
        }
        private bool CanExecuteSaveCommand()
        {
            return Title.Length >= 3;
        }
        private void DoCancelCommand()
        {
            Context.UndoChanges();
            WindowDialogService.CurrentDialogWindow.DialogResult = false;
        }
        #endregion Methods
    }
}
