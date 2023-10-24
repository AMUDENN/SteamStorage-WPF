namespace SteamStorage.Services.Config
{
    public interface IConfigService
    {
        public T GetProperty<T>(string propertyName);
        public void SetProperty(string propertyName, object value);
    }
}
