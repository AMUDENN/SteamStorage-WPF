using SteamStorage.Entities;

namespace SteamStorage.Utilities
{
    public static class Context
    {
        private static readonly SteamStorageDbContext DbContext = new();
        public static SteamStorageDbContext GetContext() => DbContext;
    }
}