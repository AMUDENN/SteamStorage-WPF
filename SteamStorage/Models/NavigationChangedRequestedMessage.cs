using CommunityToolkit.Mvvm.Messaging.Messages;

namespace SteamStorage.Models
{
    public class NavigationChangedRequestedMessage : ValueChangedMessage<NavigationModel>
    {
        #region Constructor
        public NavigationChangedRequestedMessage(NavigationModel model) : base(model) { }
        #endregion Constructor
    }
}
