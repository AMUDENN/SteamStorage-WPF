using CommunityToolkit.Mvvm.ComponentModel;

namespace SteamStorage.Services
{
    public interface IWindowDialogService
    {
        bool? ShowDialog(string title, ObservableObject dataContext);
    }
}
