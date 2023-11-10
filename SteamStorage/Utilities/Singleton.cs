using Microsoft.Extensions.DependencyInjection;

namespace SteamStorage.Utilities
{
    public static class Singleton
    {
        public static T? GetService<T>()
        {
            return App.Container.GetService<T>();
        }
    }
}
