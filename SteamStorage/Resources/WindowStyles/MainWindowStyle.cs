using System;
using System.Windows;
using System.Windows.Controls;

namespace SteamStorage.Resources.WindowStyles
{
    public partial class MainWindowStyle
    {
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            (sender as Window).StateChanged += Window_StateChanged;
        }
        private void Window_StateChanged(object sender, EventArgs e)
        {
            Window me = sender as Window;
            Button maximizeCaptionButton = me.Template.FindName("MaxRestoreButton", me) as Button;
            maximizeCaptionButton.Content = me.WindowState == WindowState.Maximized ? "2" : "1";

        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            ((sender as FrameworkElement).TemplatedParent as Window).Close();
        }
        private void MaxRestoreButton_Click(object sender, RoutedEventArgs e)
        {
            ((sender as FrameworkElement).TemplatedParent as Window)
                .WindowState = (((sender as FrameworkElement).TemplatedParent as Window)
                .WindowState == WindowState.Normal) ? WindowState.Maximized : WindowState.Normal;
        }
        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            ((sender as FrameworkElement).TemplatedParent as Window)
                .WindowState = WindowState.Minimized;
        }
    }
}
