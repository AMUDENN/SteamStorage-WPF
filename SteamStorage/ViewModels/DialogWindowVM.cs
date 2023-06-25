using CommunityToolkit.Mvvm.ComponentModel;

namespace SteamStorage.ViewModels
{
    public class DialogWindowVM : ObservableObject
    {
        #region Fields
        private string title;
        private ObservableObject currentVM;
        #endregion Fields

        #region Properties
        public string Title
        {
            get => title;
            set => SetProperty(ref title, value);
        }
        public ObservableObject CurrentVM
        {
            get => currentVM;
            set => SetProperty(ref currentVM, value);
        }
        #endregion Properties

        #region Commands
        #endregion Commands

        #region Constructor
        public DialogWindowVM(string title, ObservableObject viewModel)
        {
            Title = title;
            CurrentVM = viewModel;
        }
        #endregion Constructor

        #region Methods
        #endregion Methods
    }
}

