using System.Windows;

namespace SteamStorage.Resources.WindowStyles
{
    public partial class MessageWindowStyle
    {
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            ((sender as FrameworkElement).TemplatedParent as Window).Close();
        }
        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            ((sender as FrameworkElement).TemplatedParent as Window)
                .WindowState = WindowState.Minimized;
        }
    }
}
