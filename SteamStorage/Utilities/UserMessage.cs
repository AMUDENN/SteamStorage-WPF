using SteamStorage.Models;
using SteamStorage.Services;
using SteamStorage.ViewModels;

namespace SteamStorage.Utilities
{
    public class UserMessage
    {
        public WindowDialogService windowDialogService = new();
        public bool Question(string question)
        {
            return (bool)windowDialogService.ShowDialog("Подтверждение", new MessageBoxVM(question, MessageBoxVM.MessageImages.Question, MessageBoxVM.MessageButtons.OkCancel));
        }
        public bool Information(string info)
        {
            return (bool)windowDialogService.ShowDialog("Информация", new MessageBoxVM(info, MessageBoxVM.MessageImages.Information, MessageBoxVM.MessageButtons.Ok));
        }
        public bool Error(string error)
        {
            return (bool)windowDialogService.ShowDialog("Ошибка", new MessageBoxVM(error, MessageBoxVM.MessageImages.Error, MessageBoxVM.MessageButtons.Ok));
        }
        public bool EditArchiveGroup(ArchiveGroupModel archiveGroupModel) 
        {
            return (bool)windowDialogService.ShowDialog($"Изменение группы \"{archiveGroupModel.Title}\"", new GroupOperationsVM(archiveGroupModel));
        }
        public bool AddArchiveGroup()
        {
            return (bool)windowDialogService.ShowDialog("Добавление новой группы", new GroupOperationsVM(GroupOperationsVM.GroupTypes.Archive));
        }
        public bool EditRemainGroup(RemainGroupModel remainGroupModel)
        {
            return (bool)windowDialogService.ShowDialog($"Изменение группы \"{remainGroupModel.Title}\"", new GroupOperationsVM(remainGroupModel));
        }
        public bool AddRemainGroup()
        {
            return (bool)windowDialogService.ShowDialog("Добавление новой группы", new GroupOperationsVM(GroupOperationsVM.GroupTypes.Remain));
        }
        public bool EditArchive(ArchiveModel archiveModel)
        {
            return (bool)windowDialogService.ShowDialog(350, 550, $"Изменение элемента \"{archiveModel.Title}\"", new ArchiveEditVM(archiveModel));
        }
        public bool AddArchive(ArchiveGroupModel? archiveGroupModel)
        {
            return (bool)windowDialogService.ShowDialog(350, 550, $"Добавление элемента", new ArchiveEditVM(archiveGroupModel));
        }
        public bool EditRemain(RemainModel remainModel)
        {
            return (bool)windowDialogService.ShowDialog(350, 550, $"Изменение элемента \"{remainModel.Title}\"", new RemainEditVM(remainModel));
        }
        public bool AddRemain(RemainGroupModel? remainGroupModel)
        {
            return (bool)windowDialogService.ShowDialog(350, 550, $"Добавление элемента", new RemainEditVM(remainGroupModel));
        }
    }
}
