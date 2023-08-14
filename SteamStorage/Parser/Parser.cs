using Newtonsoft.Json;
using SteamStorage.Parser.Models;
using SteamStorage.Utilities;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace SteamStorage.Parser
{
    public static class SteamParser
    {
        #region Fields
        private static readonly HttpClient _client = new();
        private static readonly List<string> _extraChars = new() { "amp;" };
        private static readonly Logger? _logger = Singleton.GetObject<Logger>();
        #endregion Fields

        #region Methods
        public static (DateTime DateUpdate, double Price) GetCurrentSkinInfo(string url)
        {
            string result = _client.GetStringAsync($"https://steamcommunity.com/market/priceoverview/?market_hash_name={url[(url.LastIndexOf('/') + 1)..]}&appid=730&currency=5").Result;
            SkinParseModel skinParse = JsonConvert.DeserializeObject<SkinParseModel>(result);
            try
            {
                var final = (DateTime.Now, Convert.ToDouble(skinParse.lowest_price[..^4]));
                SuccessParsing();
                return final;
            }
            catch (Exception ex)
            {
                FailParsing(ex.Message);
                return (DateTime.Now, -1);
            }
        }
        public static string GetSkinTitle(string url)
        {
            try
            {
                string result = _client.GetStringAsync(url).Result;
                string slice = result[result.IndexOf(url)..];
                string final = DeleteExtraChar(slice[(url.Length + 2)..slice.IndexOf("</a>")]);
                SuccessParsing();
                return final;
            }
            catch (Exception ex)
            {
                FailParsing(ex.Message);
                return string.Empty;
            }
        }
        private static string DeleteExtraChar(string title)
        {
            foreach (string str in _extraChars)
            {
                title = title.Replace(str, string.Empty);
            }
            return title;
        }
        private static void SuccessParsing()
        {
            _logger?.WriteMessage("Успешно получена информация!", typeof(SteamParser));
        }
        private static void FailParsing(string message)
        {
            _logger?.WriteMessage($"Произошла ошибка при получении информации: {message}!", typeof(SteamParser));
        }
        #endregion Methods
    }
}