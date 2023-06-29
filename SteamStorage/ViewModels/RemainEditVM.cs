using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SteamStorage.Models;
using SteamStorage.Services;
using SteamStorage.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace SteamStorage.ViewModels
{
    public class RemainEditVM : ObservableObject
    {
        #region Fields
        private RemainModel remainModel;

        private string url;
        private string countString = string.Empty;
        private string costPurchaseString = string.Empty;

        private long count;
        private double costPurchase;
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
        public string CountString
        {
            get => countString;
            set
            {
                SetProperty(ref countString, value.Replace(".", ","));
                SaveCommand.NotifyCanExecuteChanged();
            }
        }
        public string CostPurchaseString
        {
            get => costPurchaseString;
            set
            {
                SetProperty(ref costPurchaseString, value.Replace(".", ","));
                SaveCommand.NotifyCanExecuteChanged();
            }
        }
        public long Count
        {
            get => count;
            set => SetProperty(ref count, value);
        }
        public double CostPurchase
        {
            get => costPurchase;
            set => SetProperty(ref costPurchase, value);
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
            CountString = remainModel.Count.ToString();
            CostPurchaseString = remainModel.CostPurchase.ToString();
            Groups = context.RemainGroups.ToList();
            SelectedRemainGroupModel = Groups.Where(x => x.RemainGroup == remainModel.RemainGroup).First();
        }
        public RemainEditVM(RemainGroupModel? remainGroupModel)
        {
            remainModel = new();
            Url = string.Empty;
            Groups = context.RemainGroups.ToList(); 
            SelectedRemainGroupModel = remainGroupModel is null ? Groups.First() : Groups.Where(x => x.RemainGroup == remainGroupModel.RemainGroup).First();
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
            context.UndoChanges();
            WindowDialogService.CurrentDialogWindow.DialogResult = false;
        }
        #endregion Methods
    }
}
