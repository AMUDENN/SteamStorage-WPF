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
    public class ArchiveEditVM : ObservableObject
    {
        #region Fields
        private ArchiveModel archiveModel;

        private string url;
        private long? count;
        private double? costPurchase;
        private double? costSold;
        private ArchiveGroupModel? selectedArchiveGroupModel;

        private IEnumerable<ArchiveGroupModel> groups;

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
        public double? CostSold
        {
            get => costSold;
            set
            {
                SetProperty(ref costSold, value);
                SaveCommand.NotifyCanExecuteChanged();
            }
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
        public ArchiveEditVM(ArchiveModel archiveModel)
        {
            this.archiveModel = archiveModel;
            Url = archiveModel.Url;
            Count = archiveModel.Count;
            CostPurchase = archiveModel.CostPurchase;
            CostSold = archiveModel.CostSold;
            Groups = context.ArchiveGroups.ToList();
            SelectedArchiveGroupModel = Groups.Where(x => x.ArchiveGroup == archiveModel.ArchiveGroup).First();
        }
        public ArchiveEditVM(ArchiveGroupModel? archiveGroupModel)
        {
            archiveModel = new();
            Url = string.Empty;
            Groups = context.ArchiveGroups.ToList();
            SelectedArchiveGroupModel = archiveGroupModel;
        }
        #endregion Constructor

        #region Methods
        private void DoSaveCommand()
        {
            archiveModel.EditArchive(Url, (long)Count, (double)CostPurchase, (double)CostSold, DateTime.Now, DateTime.Now, SelectedArchiveGroupModel);
            context.SaveChanges();
            WindowDialogService.CurrentDialogWindow.DialogResult = true;
        }
        private bool CanExecuteSaveCommand()
        {
            return Count is not null && CostPurchase is not null && CostSold is not null && Url.Length >= 30;
        }
        private void DoCancelCommand()
        {
            context.UndoChanges();
            WindowDialogService.CurrentDialogWindow.DialogResult = false;
        }
        #endregion Methods
    }
}
