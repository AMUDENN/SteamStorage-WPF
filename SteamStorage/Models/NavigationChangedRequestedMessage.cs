using CommunityToolkit.Mvvm.Messaging.Messages;

namespace SteamStorage.Models
{
    public class NavigationChangedRequestedMessage : ValueChangedMessage<NavigationModel>
    {
        public NavigationChangedRequestedMessage(NavigationModel model) : base(model) { }
    }
}
