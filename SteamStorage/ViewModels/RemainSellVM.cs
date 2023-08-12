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
    public class RemainSellVM : ObservableObject
    {
        #region Fields
        private RemainModel remainModel;

        private string countString = string.Empty;
        private string costSoldString = string.Empty;

        private long count;
        private double costSold;
        private ArchiveGroupModel? selectedArchiveGroupModel;

        private IEnumerable<ArchiveGroupModel> groups;

        private RelayCommand saveCommand;
        private RelayCommand cancelCommand;
        #endregion Fields

        #region Properties
        public string CountString
        {
            get => countString;
            set
            {
                SetProperty(ref countString, value.Replace(".", ","));
                SaveCommand.NotifyCanExecuteChanged();
            }
        }
        public string CostSoldString
        {
            get => costSoldString;
            set
            {
                SetProperty(ref costSoldString, value.Replace(".", ","));
                SaveCommand.NotifyCanExecuteChanged();
            }
        }
        public long Count
        {
            get => count;
            set => SetProperty(ref count, value);
        }
        public double CostSold
        {
            get => costSold;
            set => SetProperty(ref costSold, value);
        }
        public ArchiveGroupModel? SelectedArchiveGroupModel
        {
            get => selectedArchiveGroupModel;
            set
            {
                SetProperty(ref selectedArchiveGroupModel, value);
                SaveCommand.NotifyCanExecuteChanged();
            }
        }
        public IEnumerable<ArchiveGroupModel> Groups
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
        public RemainSellVM(RemainModel remainModel)
        {
            this.remainModel = remainModel;
            Count = remainModel.Count;
            CountString = remainModel.Count.ToString();
            Groups = Context.ArchiveGroups.ToList();
        }
        #endregion Constructor

        #region Methods
        private void DoSaveCommand()
        {
            remainModel.SellRemain(Count, CostSold, DateTime.Now, SelectedArchiveGroupModel);
            Context.SaveChanges();
            WindowDialogService.CurrentDialogWindow.DialogResult = true;
        }
        private bool CanExecuteSaveCommand()
        {
            try
            {
                Count = Convert.ToInt64(CountString);
                CostSold = Convert.ToDouble(CostSoldString);
                return true;
            }
            catch
            {
                return false;
            }
        }
        private void DoCancelCommand()
        {
            Context.UndoChanges();
            WindowDialogService.CurrentDialogWindow.DialogResult = false;
        }
        #endregion Methods
    }
}
