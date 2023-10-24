﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SteamStorage.Services;
using SteamStorage.Utilities;
using System.Windows;
using System.Windows.Input;

namespace SteamStorage.ViewModels
{
    public class MainWindowVM : ObservableObject
    {
        #region Fields
        private ObservableObject _currentVM;

        private RelayCommand _closingCommand;
        private RelayCommand _stateChangedCommand;
        private RelayCommand _loadedCommand;
        private RelayCommand<KeyEventArgs> _keyDownCommand;

        private readonly ReferenceInformationService? _referenceInformationService = Singleton.GetObject<ReferenceInformationService>();
        #endregion Fields

        #region Properties
        public ObservableObject CurrentVM
        {
            get => _currentVM;
            set => SetProperty(ref _currentVM, value);
        }
        #endregion Properties

        #region Commands
        public RelayCommand ClosingCommand
        {
            get
            {
                return _closingCommand ??= new RelayCommand(DoClosingCommand);
            }
        }
        public RelayCommand StateChangedCommand
        {
            get
            {
                return _stateChangedCommand ??= new RelayCommand(DoStateChangedCommand);
            }
        }
        public RelayCommand LoadedCommand
        {
            get
            {
                return _loadedCommand ??= new RelayCommand(DoLoadedCommand);
            }
        }
        public RelayCommand<KeyEventArgs> KeyDownCommand
        {
            get
            {
                return _keyDownCommand ??= new RelayCommand<KeyEventArgs>(DoKeyDownCommand);
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
            if (mw.WindowState == WindowState.Maximized) return;
            Config.Width = mw.ActualWidth;
            Config.Height = mw.ActualHeight;
            Config.Top = mw.Top;
            Config.Left = mw.Left;
        }
        private void DoLoadedCommand()
        {
            Themes.ThemesEnum currentTheme;
            if (Config.CurrentTheme == "Light") currentTheme = Themes.ThemesEnum.Light;
            else if (Config.CurrentTheme == "Dark") currentTheme = Themes.ThemesEnum.Dark;
            else currentTheme = Themes.ThemesEnum.Custom;
            Themes.ChangeTheme(currentTheme);
        }
        private void DoKeyDownCommand(KeyEventArgs? e)
        {
            if (e?.Key == Key.F1)
            {
                _referenceInformationService?.OpenReferenceInformation();
            }
        }
        #endregion Methods
    }
}

