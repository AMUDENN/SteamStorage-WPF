using CommunityToolkit.Mvvm.ComponentModel;

namespace TestSystem.Models
{
    public class NavigationModel
    {
        public required string Name { get; set; }
        public required string ImageSource { get; set; }
        public required ObservableObject DestinationVM { get; set; }
    }
}
