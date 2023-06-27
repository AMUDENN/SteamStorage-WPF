using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SteamStorage.Models;
using SteamStorage.Services;
using SteamStorage.Utilities;
using System;

namespace SteamStorage.ViewModels
{
    public class RemainEditVM : ObservableObject
    {
        #region Fields
        private RemainModel remainModel;

        private string url;
        private long count;
        private double costPurchase;
        private RemainGroupModel? selectedRemainGroupModel;

        private RelayCommand saveCommand;
        private RelayCommand cancelCommand;

        private Context context = Singleton.GetObject<Context>();
        #endregion Fields

        #region Properties
        public string Url
        {
            get => url;
            set
            {
                SetProperty(ref url, value);
                SaveCommand.NotifyCanExecuteChanged();
            }
        }
        public long Count
        {
            get => count;
            set
            {
                SetProperty(ref count, value);
                SaveCommand.NotifyCanExecuteChanged();
            }
        }
        public double CostPurchase
        {
            get => costPurchase;
            set
            {
                SetProperty(ref costPurchase, value);
                SaveCommand.NotifyCanExecuteChanged();
            }
        }
        public RemainGroupModel? SelectedRemainGroupModel
        {
            get => selectedRemainGroupModel;
            set
            {
                SetProperty(ref selectedRemainGroupModel, value);
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
        public RemainEditVM(RemainModel remainModel, RemainGroupModel remainGroupModel)
        {
            this.remainModel = remainModel;
            Url = remainModel.Url;
            Count = remainModel.Count;
            CostPurchase = remainModel.CostPurchase;
            SelectedRemainGroupModel = remainGroupModel;
        }
        public RemainEditVM(RemainGroupModel remainGroupModel)
        {
            remainModel = new();
            Url = string.Empty;
            SelectedRemainGroupModel = remainGroupModel;
        }
        #endregion Constructor

        #region Methods
        private void DoSaveCommand()
        {
            remainModel.EditRemain(Url, Count, CostPurchase, DateTime.Now, SelectedRemainGroupModel);
            context.SaveChanges();
            WindowDialogService.CurrentDialogWindow.DialogResult = true;
        }
        private bool CanExecuteSaveCommand()
        {
            return Url.Length >= 30;
        }
        private void DoCancelCommand()
        {
            context.UndoChanges();
            WindowDialogService.CurrentDialogWindow.DialogResult = false;
        }
        #endregion Methods
    }
}
