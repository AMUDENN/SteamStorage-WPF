using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SteamStorage.Models;
using SteamStorage.Services;
using SteamStorage.Utilities;
using System.Windows;

namespace SteamStorage.ViewModels
{
    public class ArchiveGroupOperationsVM : ObservableObject
    {
        #region Fields
        private ArchiveGroupModel archiveGroupModel;

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

        public ArchiveGroupOperationsVM(ArchiveGroupModel archiveGroupModel) 
        {
            this.archiveGroupModel = archiveGroupModel;
            Title = archiveGroupModel.Title;
        }
        public ArchiveGroupOperationsVM()
        {
            this.archiveGroupModel = new();
            Title = string.Empty;
        }
        #endregion Constructor

        #region Methods
        private void DoSaveCommand()
        {
            archiveGroupModel.Title = Title;
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
