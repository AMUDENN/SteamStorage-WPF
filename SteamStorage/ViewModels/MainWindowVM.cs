using CommunityToolkit.Mvvm.ComponentModel;

namespace SteamStorage.ViewModels
{
    public class MainWindowVM : ObservableObject
    {
        private ObservableObject currentVM;
        public ObservableObject CurrentVM
        {
            get => currentVM;
            set => SetProperty(ref currentVM, value);
        }
        public MainWindowVM()
        {
            CurrentVM = new MainVM();
        }
    }
}

