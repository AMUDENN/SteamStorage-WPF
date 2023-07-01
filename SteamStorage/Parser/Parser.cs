using Newtonsoft.Json;
using SteamStorage.Parser.Models;
using SteamStorage.Utilities;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace SteamStorage.Parser
{
    public class Parser
    {
        #region Fields
        private readonly HttpClient client = new();
        private readonly List<string> extraChars = new() { "amp;" };
        private readonly Logger logger = Singleton.GetObject<Logger>();
        #endregion Fields

        #region Methods
        public (DateTime DateUpdate, double Price) GetCurrentSkinInfo(string url)
        {
            string result = client.GetStringAsync($"https://steamcommunity.com/market/priceoverview/?market_hash_name={url[(url.LastIndexOf('/') + 1)..]}&appid=730&currency=5").Result;
            SkinModel skinParse = JsonConvert.DeserializeObject<SkinModel>(result);
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
        public string GetSkinTitle(string url)
        {
            try
            {
                string result = client.GetStringAsync(url).Result;
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
            foreach (string str in extraChars)
            {
                title = title.Replace(str, string.Empty);
            }
            return title;
        }
        private void SuccessParsing()
        {
            logger.WriteMessage("Успешно получена информация!", this.GetType());
        }
        private void FailParsing(string message)
        {
            logger.WriteMessage($"Произошла ошибка при получении информации: {message}!", this.GetType());
        }
        #endregion Methods
    }
}