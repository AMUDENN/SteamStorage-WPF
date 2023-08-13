using CommunityToolkit.Mvvm.ComponentModel;

namespace SteamStorage.ViewModels
{
    public class DialogWindowVM : ObservableObject
    {
        #region Fields
        private string _title;
        private ObservableObject _currentVM;
        #endregion Fields

        #region Properties
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }
        public ObservableObject CurrentVM
        {
            get => _currentVM;
            set => SetProperty(ref _currentVM, value);
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

