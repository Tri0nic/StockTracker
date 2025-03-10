﻿using StockTracker.Services.ParsersServices;
using static StockTracker.Services.ParsersServices.SeleniumService;
using static StockTracker.Parsers.Helpers.YandexMarketHelper;

namespace StockTracker.Parsers
{
    public class YandexMarketParser : IParser
    {
        public string ShopName => "Яндекс Маркет";

        private readonly ProxyService _proxyService;
        private readonly string _driverDirectory;

        public YandexMarketParser(ProxyService proxyService, string driverDirectory)
        {
            _proxyService = proxyService;
            _driverDirectory = driverDirectory;
        }

        public async Task<string> Parse(string url)
        {
            using (var driver = CreateWebDriver(_driverDirectory, _proxyService, url))
            {
                try
                {
                    if (!IsElementAvailable(driver, "//*[@id=\"/content/page/fancyPage/emptyOfferSnippet\"]/div/div/div[2]/div/div/div[1]/h2"))
                    {
                        Console.WriteLine("Нет в наличии");
                        return "Нет в наличии";
                    }
                    else
                    {
                        return CountProducts(driver).ToString();
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("Не удалось спарсить");
                    return "Не удалось спарсить";
                }
            }
        }
    }
}
