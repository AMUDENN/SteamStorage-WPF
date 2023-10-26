using Newtonsoft.Json;
using SteamStorage.Services.Logger;
using SteamStorage.Services.Parser.ParseModels;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace SteamStorage.Services.Parser
{
    public class SteamParseService : ISteamParseService
    {
        #region Fields
        private readonly HttpClient _client = new();
        private readonly List<string> _extraChars = new() { "amp;" };
        private readonly LoggerService _loggerService;
        #endregion Fields

        #region Constructor
        public SteamParseService(LoggerService logger)
        {
            _loggerService = logger;
        }
        #endregion Constructor

        #region Methods
        public (DateTime DateUpdate, double Price) GetCurrentSkinInfo(string url)
        {
            string result = _client.GetStringAsync($"https://steamcommunity.com/market/priceoverview/?market_hash_name={url[(url.LastIndexOf('/') + 1)..]}&appid=730&currency=5").Result;
            SkinPriceDynamicParseModel? skinParse = JsonConvert.DeserializeObject<SkinPriceDynamicParseModel>(result);
            try
            {
                var final = (DateTime.Now, Convert.ToDouble(skinParse?.lowest_price[..^4]));
                SuccessParsing();
                return final;
            }
            catch (Exception ex)
            {
                FailParsing(ex.Message);
                return (DateTime.Now, -1);
            }
        }
        public string GetSkinTitle(string url)
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
        private string DeleteExtraChar(string title)
        {
            foreach (string str in _extraChars)
            {
                title = title.Replace(str, string.Empty);
            }
            return title;
        }
        private void SuccessParsing()
        {
            _loggerService.WriteMessage("Успешно получена информация!", typeof(SteamParseService));
        }
        private void FailParsing(string message)
        {
            _loggerService.WriteMessage($"Произошла ошибка при получении информации: {message}!", typeof(SteamParseService));
        }
        #endregion Methods
    }
}