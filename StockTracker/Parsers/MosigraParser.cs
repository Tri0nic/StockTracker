using StockTracker.Services.ParsersServices;
using static StockTracker.Services.ParsersServices.SeleniumService;
using static StockTracker.Parsers.Helpers.MosigraHelper;

namespace StockTracker.Parsers
{
    public class MosigraParser : IParser
    {
        public string ShopName => "Мосигра";

        private readonly ProxyService _proxyService;
        private readonly string _driverDirectory;

        public MosigraParser(ProxyService proxyService, string driverDirectory)
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
                    ChooseRegion(driver);

                    if (!IsElementAvailable(driver, "//*[@id=\"app-main\"]/main/section[1]/article/section[1]/noindex/div/span[2]/b"))
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
