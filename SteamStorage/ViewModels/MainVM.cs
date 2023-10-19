using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using SteamStorage.Models;
using SteamStorage.Utilities;
using SteamStorage.Windows;

namespace SteamStorage.ViewModels
{
    public class MainVM : ObservableObject
    {
        #region Fields
        private ObservableObject _currentVM;
        private bool _isMenuExpanded;
        #endregion Fields

        #region Properties
        public ObservableObject NavigationVM { get; set; }
        public ObservableObject CurrentVM
        {
            get => _currentVM;
            set => SetProperty(ref _currentVM, value);
        }
        public bool IsMenuExpanded
        {
            get => _isMenuExpanded;
            set => SetProperty(ref _isMenuExpanded, value);
        }
        public string Version
        {
            get => Constants.Version;
        }
        #endregion Properties

        #region Constructor
        public MainVM()
        {
            IsMenuExpanded = Config.IsMenuExpanded;
            Singleton.GetObject<MainWindow>().Closing += (s, e) => Config.IsMenuExpanded = IsMenuExpanded;

            bool isGreetingTextVisible = Config.IsGreetingTextVisible;

            if (isGreetingTextVisible) CurrentVM = new WelcomeVM();

            WeakReferenceMessenger.Default.Register<NavigationChangedRequestedMessage>(this, NavigateTo);
            NavigationVM = new NavigationVM(!isGreetingTextVisible);
        }
        #endregion Constructor

        #region Methods
        private void NavigateTo(object recipient, NavigationChangedRequestedMessage message)
        {
            if (message.Value is NavigationModel navModel)
                CurrentVM = navModel.DestinationVM;
        }
        #endregion Methods
    }
}

