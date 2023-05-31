using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using SteamStorage.Utilities;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TestSystem.Models;

namespace SteamStorage.ViewModels
{
    public class NavigationVM : ObservableObject
    {
        public List<NavigationModel> NavigationOptions { get; set; } = new List<NavigationModel>();

        public ICommand SelectionChangedCommand { get; set; } = new RelayCommand<object>((o) =>
        {
            if (o is SelectionChangedEventArgs selectionChanged)
            {
                if (selectionChanged.AddedItems.Count == 0)
                    return;
                if (selectionChanged.AddedItems[0] is NavigationModel navModel)
                {
                    var message = new NavigationChangedRequestedMessage(navModel);
                    WeakReferenceMessenger.Default.Send(message);
                }
            }
        });
        public NavigationVM()
        {
            NavigationOptions.Add(new NavigationModel()
            {
                Name = "Главная",
                ImageStyle = Styles.GetStyle("DiagramImage"),
                DestinationVM = new HomeVM()
            });
            NavigationOptions.Add(new NavigationModel()
            {
                Name = "Остатки",
                ImageStyle = Styles.GetStyle("DollarImage"),
                DestinationVM = new RemainsVM()
            });
            NavigationOptions.Add(new NavigationModel()
            {
                Name = "Архив",
                ImageStyle = Styles.GetStyle("CubeImage"),
                DestinationVM = new ArchiveVM()
            });
            NavigationOptions.Add(new NavigationModel()
            {
                Name = "Настройки",
                ImageStyle = Styles.GetStyle("SettingsImage"),
                DestinationVM = new SettingsVM()
            });

            var message = new NavigationChangedRequestedMessage(NavigationOptions[0]);
            WeakReferenceMessenger.Default.Send(message);
        }
    }
}

