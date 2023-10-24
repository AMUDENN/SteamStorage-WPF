using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows;

namespace SteamStorage.Models
{
    public class NavigationModel
    {
        #region Fields
        public required string Name { get; set; }
        public required Style? ImageStyle { get; set; }
        public required ObservableObject DestinationVM { get; set; }
        #endregion Fields
    }
}
