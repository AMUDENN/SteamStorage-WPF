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
            Config.Width = Application.Current.MainWindow.ActualWidth;
            Config.Height = Application.Current.MainWindow.ActualHeight;
        }
        #endregion Methods
    }
}

