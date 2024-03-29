﻿using SteamStorage.Models.EntityModels;
using SteamStorage.Services.Dialog;
using SteamStorage.ViewModels;

namespace SteamStorage.Utilities
{
    public static class UserMessage
    {
        private static readonly WindowDialogService? _windowDialogService = Singleton.GetService<WindowDialogService>();
        public static bool Question(string question, double height = 200, double width = 400)
        {
            return (bool)_windowDialogService.ShowDialog(height, width, "Подтверждение", new MessageBoxVM(question, MessageBoxVM.MessageImages.Question, MessageBoxVM.MessageButtons.OkCancel));
        }
        public static bool Information(string info, double height = 200, double width = 400)
        {
            return (bool)_windowDialogService.ShowDialog(height, width, "Информация", new MessageBoxVM(info, MessageBoxVM.MessageImages.Information, MessageBoxVM.MessageButtons.Ok));
        }
        public static bool Error(string error, double height = 200, double width = 400)
        {
            return (bool)_windowDialogService.ShowDialog(height, width, "Ошибка", new MessageBoxVM(error, MessageBoxVM.MessageImages.Error, MessageBoxVM.MessageButtons.Ok));
        }
        public static bool TextConfirmation(string text, string confirmationWord, double height = 350, double width = 500)
        {
            return (bool)_windowDialogService.ShowDialog(height, width, "Подтвердите действие", new TextConfirmationVM(text, confirmationWord));
        }
        public static bool EditArchiveGroup(ArchiveGroupElementModel archiveGroupModel, double height = 250, double width = 400) 
        {
            return (bool)_windowDialogService.ShowDialog(height, width, $"Изменение группы \"{archiveGroupModel.Title}\"", new GroupOperationsVM(archiveGroupModel));
        }
        public static bool AddArchiveGroup(double height = 250, double width = 400)
        {
            return (bool)_windowDialogService.ShowDialog(height, width, "Добавление новой группы", new GroupOperationsVM(GroupOperationsVM.GroupTypes.Archive));
        }
        public static bool EditRemainGroup(RemainGroupElementModel remainGroupModel, double height = 250, double width = 400)
        {
            return (bool)_windowDialogService.ShowDialog(height, width, $"Изменение группы \"{remainGroupModel.Title}\"", new GroupOperationsVM(remainGroupModel));
        }
        public static bool AddRemainGroup(double height = 250, double width = 400)
        {
            return (bool)_windowDialogService.ShowDialog(height, width, "Добавление новой группы", new GroupOperationsVM(GroupOperationsVM.GroupTypes.Remain));
        }
        public static bool EditArchive(ArchiveElementModel archiveModel, double height = 400, double width = 550)
        {
            return (bool)_windowDialogService.ShowDialog(height, width, $"Изменение элемента \"{archiveModel.Title}\"", new ArchiveEditVM(archiveModel));
        }
        public static bool AddArchive(ArchiveGroupElementModel? archiveGroupModel, double height = 350, double width = 550)
        {
            return (bool)_windowDialogService.ShowDialog(height, width, $"Добавление элемента", new ArchiveEditVM(archiveGroupModel));
        }
        public static bool EditRemain(RemainElementModel remainModel, double height = 400, double width = 550)
        {
            return (bool)_windowDialogService.ShowDialog(height, width, $"Изменение элемента \"{remainModel.Title}\"", new RemainEditVM(remainModel));
        }
        public static bool AddRemain(RemainGroupElementModel? remainGroupModel, double height = 350, double width = 550)
        {
            return (bool)_windowDialogService.ShowDialog(height, width, $"Добавление элемента", new RemainEditVM(remainGroupModel));
        }
        public static bool SellRemain(RemainElementModel remainModel, double height = 300, double width = 450) 
        {
            return (bool)_windowDialogService.ShowDialog(height, width, $"Продажа элемента \"{remainModel.Title}\"", new RemainSellVM(remainModel));
        }
    }
}
