using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using SteamStorage.Models;
using SteamStorage.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace SteamStorage.ViewModels
{
    public class NavigationVM : ObservableObject
    {
        #region Fields
        private readonly List<NavigationModel> _navigationOptions = new()
        {
            new NavigationModel()
            {
                Name = "Главная",
                ImageStyle = Dictionaries.GetStyle("DiagramImage"),
                DestinationVM = new HomeVM()
            },
            new NavigationModel()
            {
                Name = "Остатки",
                ImageStyle = Dictionaries.GetStyle("DollarImage"),
                DestinationVM = new RemainsVM()
            },
            new NavigationModel()
            {
                Name = "Архив",
                ImageStyle = Dictionaries.GetStyle("CubeImage"),
                DestinationVM = new ArchiveVM()
            },
            new NavigationModel()
            {
                Name = "Настройки",
                ImageStyle = Dictionaries.GetStyle("SettingsImage"),
                DestinationVM = new SettingsVM()
            }
        };
        private NavigationModel _selectedNavigationModel;

        private RelayCommand<object> _selectionChangedCommand;
        #endregion Fields

        #region Properties
        public List<NavigationModel> NavigationOptions => _navigationOptions;
        public NavigationModel SelectedNavigationModel
        {
            get => _selectedNavigationModel;
            set => SetProperty(ref _selectedNavigationModel, value);
        }
        #endregion Properties

        #region Commands
        public RelayCommand<object> SelectionChangedCommand
        {
            get
            {
                return _selectionChangedCommand ??= new RelayCommand<object>(DoSelectionChangedCommand);
            }
        }
        #endregion Commands

        #region Constructor
        public NavigationVM(bool isFirstOpen = false)
        {
            if (isFirstOpen) ChangeVM(NavigationOptions.First());
        }
        #endregion Constructor

        #region Methods
        private void DoSelectionChangedCommand(object? data)
        {
            if (data is SelectionChangedEventArgs selectionChanged)
            {
                if (selectionChanged.AddedItems.Count == 0)
                    return;
                if (selectionChanged.AddedItems[0] is NavigationModel navModel)
                {
                    ChangeVM(navModel);
                }
            }
        }
        private void ChangeVM(NavigationModel navModel)
        {
            SelectedNavigationModel = navModel;
            var message = new NavigationChangedRequestedMessage(navModel);
            WeakReferenceMessenger.Default.Send(message);
        }
        #endregion Methods
    }
}

