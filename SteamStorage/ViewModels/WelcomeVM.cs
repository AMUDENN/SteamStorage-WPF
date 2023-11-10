using CommunityToolkit.Mvvm.ComponentModel;
using SteamStorage.Services.Config;
using SteamStorage.Utilities;

namespace SteamStorage.ViewModels
{
    public class WelcomeVM : ObservableObject
    {
        #region Fields
        private bool _isGreetingTextVisible;
        private bool _isDontShowAgainEnabled;

        private readonly ConfigService? _configService = Singleton.GetService<ConfigService>();
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
            IsGreetingTextVisible = _configService.IsGreetingTextVisible;
            IsDontShowAgainEnabled = !IsGreetingTextVisible;
        }
        #endregion Constructor

        #region Methods
        private void SaveGreetingTextVisible()
        {
            _configService.IsGreetingTextVisible = !IsDontShowAgainEnabled;
        }
        #endregion Methods
    }
}
