using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SteamStorage.Utilities;

namespace SteamStorage.ViewModels
{
    public class WelcomeVM : ObservableObject
    {
        #region Fields
        private bool isGreetingTextVisible;
        private bool isDontShowAgainEnabled;
        #endregion Fields

        #region Properties
        public bool IsGreetingTextVisible
        {
            get => isGreetingTextVisible;
            set => SetProperty(ref isGreetingTextVisible, value);
        }
        public bool IsDontShowAgainEnabled
        {
            get => isDontShowAgainEnabled;
            set
            {
                SetProperty(ref isDontShowAgainEnabled, value);
                SaveGreetingTextVisible();
            }
        }
        #endregion Properties

        #region Constructor
        public WelcomeVM()
        {
            IsGreetingTextVisible = Config.IsGreetingTextVisible;
            IsDontShowAgainEnabled = false;
        }
        #endregion Constructor

        #region Methods
        private void SaveGreetingTextVisible()
        {
            Config.IsGreetingTextVisible = !IsDontShowAgainEnabled;
        }
        #endregion Methods
    }
}
