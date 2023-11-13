using System;

namespace SteamStorage.Services.Config
{
    public interface IConfigService
    {
        public T GetProperty<T>(string propertyName);
        public T GetEnumProperty<T>(string propertyName) where T : Enum;
        public void SetProperty(string propertyName, object value);
    }
}
