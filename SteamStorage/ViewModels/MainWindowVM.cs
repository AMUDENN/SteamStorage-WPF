using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SteamStorage.Utilities;
using System.Windows;

namespace SteamStorage.ViewModels
{
    public class MainWindowVM : ObservableObject
    {
        #region Fields
        private ObservableObject currentVM;
        private RelayCommand closingCommand;
        private RelayCommand stateChangedCommand;
        private RelayCommand loadedCommand;
        #endregion Fields

        #region Properties
        public ObservableObject CurrentVM
        {
            get => currentVM;
            set => SetProperty(ref currentVM, value);
        }
        #endregion Properties

        #region Commands
        public RelayCommand ClosingCommand
        {
            get
            {
                return closingCommand ??= new RelayCommand(DoClosingCommand);
            }
        }
        public RelayCommand StateChangedCommand
        {
            get
            {
                return stateChangedCommand ??= new RelayCommand(DoStateChangedCommand);
            }
        }
        public RelayCommand LoadedCommand
        {
            get
            {
                return loadedCommand ??= new RelayCommand(DoLoadedCommand);
            }
        }
        #endregion Commands

        #region Constructor
        public MainWindowVM()
        {
            CurrentVM = new MainVM();
        }
        #endregion Constructor

        #region Methods
        private void DoClosingCommand()
        {
            var mw = Application.Current.MainWindow;

            bool isMaximized = mw.WindowState == WindowState.Maximized;
            Config.IsMaximized = isMaximized;
            if (isMaximized) return;

            Config.Width = mw.ActualWidth;
            Config.Height = mw.ActualHeight;
            Config.Top = mw.Top;
            Config.Left = mw.Left;
            
        }
        private void DoStateChangedCommand()
        {
            var mw = Application.Current.MainWindow;
            Config.Width = mw.ActualWidth;
            Config.Height = mw.ActualHeight;
            Config.Top = mw.Top;
            Config.Left = mw.Left;
        }
        private void DoLoadedCommand()
        {
            if (Config.CurrentTheme == "Light") Themes.ChangeTheme(Themes.ThemesEnum.Light);
            else if (Config.CurrentTheme == "Dark") Themes.ChangeTheme(Themes.ThemesEnum.Dark);
            else if (Config.CurrentTheme == "Custom")
            {
                Themes.ChangeTheme(Themes.ThemesEnum.Custom);
            }
        }
        #endregion Methods
    }
}

