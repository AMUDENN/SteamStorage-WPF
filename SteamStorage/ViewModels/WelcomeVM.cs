using CommunityToolkit.Mvvm.ComponentModel;
using SteamStorage.Utilities;

namespace SteamStorage.ViewModels
{
    public class WelcomeVM : ObservableObject
    {
        #region Fields
        private bool _isGreetingTextVisible;
        private bool _isDontShowAgainEnabled;
        #endregion Fields

        #region Properties
        public bool IsGreetingTextVisible
        {
            get => _isGreetingTextVisible;
            set => SetProperty(ref _isGreetingTextVisible, value);
        }
        public bool IsDontShowAgainEnabled
        {
            get => _isDontShowAgainEnabled;
            set
            {
                SetProperty(ref _isDontShowAgainEnabled, value);
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
