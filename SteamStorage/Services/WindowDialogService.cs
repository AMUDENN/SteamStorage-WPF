﻿using CommunityToolkit.Mvvm.ComponentModel;
using SteamStorage.ViewModels;
using SteamStorage.Windows;

namespace SteamStorage.Services
{
    public interface IWindowDialogService
    {
        bool? ShowDialog(string title, ObservableObject dataContext);
    }
    public class WindowDialogService : IWindowDialogService
    {
        public static DialogWindow CurrentDialogWindow;
        public bool? ShowDialog(string title, ObservableObject dataContext)
        {
            DialogWindow dialogWindow = new();
            CurrentDialogWindow = dialogWindow;
            dialogWindow.DataContext = new DialogWindowVM(title, dataContext);
            return dialogWindow.ShowDialog();
        }
    }
}
