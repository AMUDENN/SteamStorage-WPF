using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SteamStorage.Models;
using SteamStorage.Services;
using SteamStorage.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SteamStorage.ViewModels
{
    public class RemainEditVM : ObservableObject
    {
        #region Fields
        private RemainModel remainModel;

        private string url;
        private long? count = null;
        private double? costPurchase = null;
        private RemainGroupModel? selectedRemainGroupModel;

        private IEnumerable<RemainGroupModel> groups;

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
        public long? Count
        {
            get => count;
            set
            {
                SetProperty(ref count, value);
                SaveCommand.NotifyCanExecuteChanged();
            }
        }
        public double? CostPurchase
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
        public IEnumerable<RemainGroupModel> Groups
        {
            get => groups;
            set => SetProperty(ref groups, value);
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
        public RemainEditVM(RemainModel remainModel)
        {
            this.remainModel = remainModel;
            Url = remainModel.Url;
            Count = remainModel.Count;
            CostPurchase = remainModel.CostPurchase;
            Groups = context.RemainGroups.ToList();
            SelectedRemainGroupModel = Groups.Where(x => x.RemainGroup == remainModel.RemainGroup).First();
        }
        public RemainEditVM(RemainGroupModel remainGroupModel)
        {
            remainModel = new();
            Url = string.Empty;
            Groups = context.RemainGroups.ToList(); 
            SelectedRemainGroupModel = remainGroupModel;
        }
        #endregion Constructor

        #region Methods
        private void DoSaveCommand()
        {
            remainModel.EditRemain(Url, (long)Count, (double)CostPurchase, DateTime.Now, SelectedRemainGroupModel);
            context.SaveChanges();
            WindowDialogService.CurrentDialogWindow.DialogResult = true;
        }
        private bool CanExecuteSaveCommand()
        {
            return Count is not null && CostPurchase is not null && Url.Length >= 30;
        }
        private void DoCancelCommand()
        {
            context.UndoChanges();
            WindowDialogService.CurrentDialogWindow.DialogResult = false;
        }
        #endregion Methods
    }
}
