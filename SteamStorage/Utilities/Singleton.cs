using Microsoft.Extensions.DependencyInjection;

namespace SteamStorage.Utilities
{
    public static class Singleton
    {
        public static T? GetObject<T>()
        {
            return App.Container.GetService<T>();
        }
    }
}
