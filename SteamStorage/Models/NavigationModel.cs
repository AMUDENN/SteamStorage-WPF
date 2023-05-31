using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows;

namespace TestSystem.Models
{
    public class NavigationModel
    {
        public required string Name { get; set; }
        public required Style ImageStyle { get; set; }
        public required ObservableObject DestinationVM { get; set; }
    }
}
