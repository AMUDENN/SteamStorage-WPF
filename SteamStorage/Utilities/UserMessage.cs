using SteamStorage.Services;
using SteamStorage.ViewModels;

namespace SteamStorage.Utilities
{
    public class UserMessage
    {
        public static WindowDialogService windowDialogService = new();
        public static bool? Question(string question)
        {
            return windowDialogService.ShowDialog("Подтверждение", new MessageBoxVM(question, MessageBoxVM.MessageImages.Question, MessageBoxVM.MessageButtons.OkCancel));
        }
        public static bool? Information(string info)
        {
            return windowDialogService.ShowDialog("Информация", new MessageBoxVM(info, MessageBoxVM.MessageImages.Information, MessageBoxVM.MessageButtons.Ok));
        }
        public static bool? Error(string error)
        {
            return windowDialogService.ShowDialog("Ошибка", new MessageBoxVM(error, MessageBoxVM.MessageImages.Error, MessageBoxVM.MessageButtons.Ok));
        }
    }
}
