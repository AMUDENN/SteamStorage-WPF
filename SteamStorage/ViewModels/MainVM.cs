using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using TestSystem.Models;

namespace SteamStorage.ViewModels
{
    public class MainVM : ObservableObject
    {
        public ObservableObject NavigationVM { get; set; }
        private ObservableObject currentVM;
        public ObservableObject CurrentVM
        {
            get => currentVM;
            set => SetProperty(ref currentVM, value);
        }
        public MainVM()
        {
            NavigationVM = new NavigationVM();
            WeakReferenceMessenger.Default.Register<NavigationChangedRequestedMessage>(this, NavigateTo);
        }
        private void NavigateTo(object recipient, NavigationChangedRequestedMessage message)
        {
            if (message.Value is NavigationModel navModel)
                CurrentVM = navModel.DestinationVM;
        }
    }
}

