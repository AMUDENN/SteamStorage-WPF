﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SteamStorage.Services.Config;
using SteamStorage.Services.ReferenceInformation;
using SteamStorage.Utilities;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using static SteamStorage.Utilities.Themes;

namespace SteamStorage.ViewModels
{
    public class MainWindowVM : ObservableObject
    {
        #region Fields
        private ObservableObject _currentVM;

        private RelayCommand<CancelEventArgs> _closingCommand;
        private RelayCommand _stateChangedCommand;
        private RelayCommand _loadedCommand;
        private RelayCommand<KeyEventArgs> _keyDownCommand;

        private readonly ConfigService? _configService = Singleton.GetService<ConfigService>();
        private readonly ReferenceInformationService? _referenceInformationService = Singleton.GetService<ReferenceInformationService>();
        #endregion Fields

        #region Properties
        public string? Title
        {
            get => ProgramConstants.Title;
        }
        public ObservableObject CurrentVM
        {
            get => _currentVM;
            set => SetProperty(ref _currentVM, value);
        }
        #endregion Properties

        #region Commands
        public RelayCommand<CancelEventArgs> ClosingCommand
        {
            get
            {
                return _closingCommand ??= new RelayCommand<CancelEventArgs>(DoClosingCommand);
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
        private void DoClosingCommand(CancelEventArgs? e)
        {
            bool close = UserMessage.Question("Вы уверены, что хотите закрыть программу?");
            if (e is null) return;
            if (!close) e.Cancel = true;

            var mw = Application.Current.MainWindow;

            bool isMaximized = mw.WindowState == WindowState.Maximized;
            _configService.IsMaximized = isMaximized;
            if (isMaximized) return;

            _configService.Width = mw.ActualWidth;
            _configService.Height = mw.ActualHeight;
            _configService.Top = mw.Top;
            _configService.Left = mw.Left;
        }
        private void DoStateChangedCommand()
        {
            var mw = Application.Current.MainWindow;
            if (mw.WindowState == WindowState.Maximized) return;
            _configService.Width = mw.ActualWidth;
            _configService.Height = mw.ActualHeight;
            _configService.Top = mw.Top;
            _configService.Left = mw.Left;
        }
        private void DoLoadedCommand()
        {
            ChangeTheme(_configService.CurrentTheme);
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

