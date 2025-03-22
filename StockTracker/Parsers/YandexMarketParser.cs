using StockTracker.Services.ParsersServices;
using static StockTracker.Services.ParsersServices.SeleniumService;
using static StockTracker.Parsers.Helpers.YandexMarketHelper;

namespace StockTracker.Parsers
{
    public class YandexMarketParser : IParser
    {
        public string ShopName => "Яндекс Маркет";

        private readonly ProxyService _proxyService;
        private readonly string _driverDirectory;
        private readonly ILogger<YandexMarketParser> _logger;

        public YandexMarketParser(ProxyService proxyService, string driverDirectory, ILogger<YandexMarketParser> logger)
        {
            _proxyService = proxyService;
            _driverDirectory = driverDirectory;
            _logger = logger;
        }

        public async Task<string> Parse(string url)
        {
            using (var driver = CreateWebDriver(_driverDirectory, _proxyService, url))
            {
                try
                {
                    _logger.LogInformation("Начат парсинг страницы: {Url}", url);

                    if (!IsElementAvailable(driver, "//*[@id=\"/content/page/fancyPage/emptyOfferSnippet\"]/div/div/div[2]/div/div/div[1]/h2"))
                    {
                        _logger.LogWarning("Товар отсутствует в наличии: {Url}", url);
                        return "Нет в наличии";
                    }
                    else
                    {
                        int count = CountProducts(driver, _logger);
                        _logger.LogInformation("Успешно получено количество товара: {Count} для {Url}", count, url);
                        return count.ToString();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ошибка при парсинге страницы: {Url}", url);
                    return "Не удалось спарсить";
                }
            }
        }
    }
}
