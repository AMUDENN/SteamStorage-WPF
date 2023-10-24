namespace SteamStorage.Services
{
    public interface IFileDialogService
    {
        string FilePath { get; set; }
        bool OpenFileDialog(string filter = "");
        bool SaveFileDialog(string filter = "");
    }
}
