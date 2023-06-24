using SteamStorage.ViewModels;
using SteamStorage.Windows;

namespace SteamStorage.Utilities
{
    public class UserMessage
    {
        public static bool? ActionConfirmation(string question)
        {
            MessageWindow messageWindow = new MessageWindow();
            messageWindow.DataContext = new MessageWindowVM("Подтверждение", new WelcomeVM());
            return messageWindow.ShowDialog();
        }
        public static bool? Information(string info)
        {
            MessageWindow messageWindow = new MessageWindow();
            messageWindow.DataContext = new MessageWindowVM("Информация", new WelcomeVM());
            return messageWindow.ShowDialog();
        }
        public static bool? Error(string text)
        {
            MessageWindow messageWindow = new MessageWindow();
            messageWindow.DataContext = new MessageWindowVM("Ошибка", new WelcomeVM());
            return messageWindow.ShowDialog();
        }
    }
}
