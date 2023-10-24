using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Win32;
using SteamStorage.ViewModels;
using SteamStorage.Windows;

namespace SteamStorage.Services
{
    public class WindowDialogService : IWindowDialogService, IFileDialogService
    {
        public static DialogWindow CurrentDialogWindow;
        public string FilePath { get; set; }
        public bool? ShowDialog(string title, ObservableObject dataContext)
        {
            DialogWindow dialogWindow = new();
            CurrentDialogWindow = dialogWindow;
            dialogWindow.DataContext = new DialogWindowVM(title, dataContext);
            return dialogWindow.ShowDialog();
        }
        public bool? ShowDialog(double height, double width, string title, ObservableObject dataContext)
        {
            DialogWindow dialogWindow = new();
            CurrentDialogWindow = dialogWindow;
            dialogWindow.DataContext = new DialogWindowVM(title, dataContext);
            dialogWindow.Height = height;
            dialogWindow.Width = width;
            return dialogWindow.ShowDialog();
        }
        public bool OpenFileDialog(string filter = "")
        {
            OpenFileDialog openFileDialog = new()
            {
                Filter = filter
            };
            if (openFileDialog.ShowDialog() == true)
            {
                FilePath = openFileDialog.FileName;
                return true;
            }
            return false;
        }
        public bool SaveFileDialog(string filter = "")
        {
            SaveFileDialog saveFileDialog = new()
            {
                Filter = filter
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                FilePath = saveFileDialog.FileName;
                return true;
            }
            return false;
        }
    }
}
