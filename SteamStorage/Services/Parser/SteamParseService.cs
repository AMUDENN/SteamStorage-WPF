﻿using Newtonsoft.Json;
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
            try
            {
                string gameIdstr = url[..url.LastIndexOf('/')];
                string result = _client.GetStringAsync($"https://steamcommunity.com/market/priceoverview/?market_hash_name=" +
                    $"{url[(url.LastIndexOf('/') + 1)..]}&appid={gameIdstr[(gameIdstr.LastIndexOf('/') + 1)..]}&currency=5").Result;

                SkinPriceDynamicParseModel? skinParse = JsonConvert.DeserializeObject<SkinPriceDynamicParseModel>(result);
                var final = (DateTime.Now, Convert.ToDouble(skinParse?.lowest_price[..^4]));
                SuccessParsing();
                return final;
            }
            catch (Exception ex)
            {
                FailParsing(ex);
                return (DateTime.Now, -1);
            }
        }
        public string GetSkinTitle(string url)
        {
            try
            {
                if (url.Contains("?filter=")) url = url[..url.LastIndexOf("?filter")];
                string result = _client.GetStringAsync(url).Result;
                string slice = result[result.IndexOf(url)..];
                string final = DeleteExtraChar(slice[(url.Length + 2)..slice.IndexOf("</a>")]);
                SuccessParsing();
                return final;
            }
            catch (Exception ex)
            {
                FailParsing(ex);
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
        private void FailParsing(Exception exception)
        {
            _loggerService.WriteMessage(exception, $"Произошла ошибка при получении информации!");
        }
        #endregion Methods
    }
}