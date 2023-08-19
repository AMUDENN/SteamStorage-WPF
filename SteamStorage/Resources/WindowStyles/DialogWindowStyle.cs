using System.Windows;

namespace SteamStorage.Resources.WindowStyles
{
    public partial class DialogWindowStyle
    {
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            ((sender as FrameworkElement)?.TemplatedParent as Window)?.Close();
        }
        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            Window? me = (sender as FrameworkElement)?.TemplatedParent as Window;
            if (me is not null)
                me.WindowState = WindowState.Minimized;
        }
    }
}
