﻿using SteamStorage.Models;
using SteamStorage.Services;
using SteamStorage.ViewModels;

namespace SteamStorage.Utilities
{
    public static class UserMessage
    {
        private static readonly WindowDialogService _windowDialogService = new();
        public static bool Question(string question)
        {
            return (bool)_windowDialogService.ShowDialog("Подтверждение", new MessageBoxVM(question, MessageBoxVM.MessageImages.Question, MessageBoxVM.MessageButtons.OkCancel));
        }
        public static bool Information(string info)
        {
            return (bool)_windowDialogService.ShowDialog("Информация", new MessageBoxVM(info, MessageBoxVM.MessageImages.Information, MessageBoxVM.MessageButtons.Ok));
        }
        public static bool Error(string error)
        {
            return (bool)_windowDialogService.ShowDialog("Ошибка", new MessageBoxVM(error, MessageBoxVM.MessageImages.Error, MessageBoxVM.MessageButtons.Ok));
        }
        public static bool EditArchiveGroup(ArchiveGroupModel archiveGroupModel) 
        {
            return (bool)_windowDialogService.ShowDialog($"Изменение группы \"{archiveGroupModel.Title}\"", new GroupOperationsVM(archiveGroupModel));
        }
        public static bool AddArchiveGroup()
        {
            return (bool)_windowDialogService.ShowDialog("Добавление новой группы", new GroupOperationsVM(GroupOperationsVM.GroupTypes.Archive));
        }
        public static bool EditRemainGroup(RemainGroupModel remainGroupModel)
        {
            return (bool)_windowDialogService.ShowDialog($"Изменение группы \"{remainGroupModel.Title}\"", new GroupOperationsVM(remainGroupModel));
        }
        public static bool AddRemainGroup()
        {
            return (bool)_windowDialogService.ShowDialog("Добавление новой группы", new GroupOperationsVM(GroupOperationsVM.GroupTypes.Remain));
        }
        public static bool EditArchive(ArchiveElementModel archiveModel)
        {
            return (bool)_windowDialogService.ShowDialog(350, 550, $"Изменение элемента \"{archiveModel.Title}\"", new ArchiveEditVM(archiveModel));
        }
        public static bool AddArchive(ArchiveGroupModel? archiveGroupModel)
        {
            return (bool)_windowDialogService.ShowDialog(350, 550, $"Добавление элемента", new ArchiveEditVM(archiveGroupModel));
        }
        public static bool EditRemain(RemainElementModel remainModel)
        {
            return (bool)_windowDialogService.ShowDialog(350, 550, $"Изменение элемента \"{remainModel.Title}\"", new RemainEditVM(remainModel));
        }
        public static bool AddRemain(RemainGroupModel? remainGroupModel)
        {
            return (bool)_windowDialogService.ShowDialog(350, 550, $"Добавление элемента", new RemainEditVM(remainGroupModel));
        }
        public static bool SellRemain(RemainElementModel remainModel) 
        {
            return (bool)_windowDialogService.ShowDialog(250, 450, $"Продажа элемента \"{remainModel.Title}\"", new RemainSellVM(remainModel));
        }
    }
}
