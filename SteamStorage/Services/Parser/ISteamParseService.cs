using System;

namespace SteamStorage.Services.Parser
{
    public interface ISteamParseService
    {
        public (DateTime DateUpdate, double Price) GetCurrentSkinInfo(string url);
        public string GetSkinTitle(string url);
    }
}
