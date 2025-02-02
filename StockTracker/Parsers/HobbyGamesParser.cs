using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using StockTracker.Services.ParsersServices;
using static StockTracker.Services.ParsersServices.JavaScriptService;
using static StockTracker.Services.ParsersServices.SeleniumService;

namespace StockTracker.Parsers
{
    public class HobbyGamesParser : IParser
    {
        public string ShopName => "Hobby Games";

        private readonly ProxyService _proxyService;

        public HobbyGamesParser(ProxyService proxyService)
        {
            _proxyService = proxyService;

        }

        public async Task<string> Parse(string url)
        {
            using (var driver = CreateWebDriver(_proxyService, url))
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

        private string CountProducts(IWebDriver driver)
        {
            return "0";
        }
    }
}
