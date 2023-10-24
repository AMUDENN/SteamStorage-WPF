using CommunityToolkit.Mvvm.ComponentModel;

namespace SteamStorage.Services.Dialog
{
    public interface IWindowDialogService
    {
        bool? ShowDialog(string title, ObservableObject dataContext);
    }
}
