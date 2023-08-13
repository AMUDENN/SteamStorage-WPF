using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using SteamStorage.Models;
using SteamStorage.Utilities;
using System.Collections.Generic;
using System.Windows.Controls;

namespace SteamStorage.ViewModels
{
    public class NavigationVM : ObservableObject
    {
        #region Fields
        private readonly List<NavigationModel> navigationOptions = new()
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
        private RelayCommand<object> selectionChangedCommand;
        #endregion Fields

        #region Properties
        public List<NavigationModel> NavigationOptions => navigationOptions;
        #endregion Properties

        #region Commands
        public RelayCommand<object> SelectionChangedCommand
        {
            get
            {
                return selectionChangedCommand ??= new RelayCommand<object>(DoSelectionChangedCommand);
            }
        }
        #endregion Commands

        #region Constructor
        public NavigationVM()
        {
            
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
                    var message = new NavigationChangedRequestedMessage(navModel);
                    WeakReferenceMessenger.Default.Send(message);
                }
            }
        }
        #endregion Methods
    }
}

