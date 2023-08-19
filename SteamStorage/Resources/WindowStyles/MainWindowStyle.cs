using System;
using System.Windows;
using System.Windows.Controls;

namespace SteamStorage.Resources.WindowStyles
{
    public partial class MainWindowStyle
    {
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Window? me = sender as Window;
            if (me is not null)
                me.StateChanged += Window_StateChanged;
        }
        private void Window_StateChanged(object? sender, EventArgs e)
        {
            Window? me = sender as Window;
            Button? maximizeCaptionButton = me?.Template.FindName("MaxRestoreButton", me) as Button;
            if(maximizeCaptionButton is not null)
                maximizeCaptionButton.Content = me?.WindowState == WindowState.Maximized ? "2" : "1";

        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            ((sender as FrameworkElement)?.TemplatedParent as Window)?.Close();
        }
        private void MaxRestoreButton_Click(object sender, RoutedEventArgs e)
        {
            Window? me = (sender as FrameworkElement)?.TemplatedParent as Window;
            if (me is not null)
                me.WindowState = me.WindowState == WindowState.Normal ? WindowState.Maximized : WindowState.Normal;
        }
        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            Window? me = (sender as FrameworkElement)?.TemplatedParent as Window;
            if (me is not null)
                me.WindowState = WindowState.Minimized;
        }
    }
}
