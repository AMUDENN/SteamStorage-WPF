using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using SteamStorage.Models;

namespace SteamStorage.ViewModels
{
    public class MainVM : ObservableObject
    {
        #region Fields
        private ObservableObject currentVM = new GreetingsVM();
        #endregion Fields

        #region Properties
        public ObservableObject NavigationVM { get; set; }
        public ObservableObject CurrentVM
        {
            get => currentVM;
            set => SetProperty(ref currentVM, value);
        }
        #endregion Properties

        #region Constructor
        public MainVM()
        {
            NavigationVM = new NavigationVM();
            WeakReferenceMessenger.Default.Register<NavigationChangedRequestedMessage>(this, NavigateTo);
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

