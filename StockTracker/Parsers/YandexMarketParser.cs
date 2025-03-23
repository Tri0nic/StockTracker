using StockTracker.Services.ParsersServices;
using static StockTracker.Services.ParsersServices.SeleniumService;
using static StockTracker.Parsers.Helpers.YandexMarketHelper;
using StockTracker.Models;

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

        public async Task<string> Parse(Product product)
        {
            using (var driver = CreateWebDriver(_driverDirectory, _proxyService, product.Link))
            {
                try
                {
                    _logger.LogInformation($"Начат парсинг товара: {product.Shop} --- {product.ProductName}");

                    if (!IsElementAvailable(driver, "//*[@id=\"/content/page/fancyPage/emptyOfferSnippet\"]/div/div/div[2]/div/div/div[1]/h2"))
                    {
                        _logger.LogWarning($"Товар отсутствует в наличии: {product.Shop} --- {product.ProductName}");
                        return "Нет в наличии";
                    }
                    else
                    {
                        string count = CountProducts(driver, _logger);
                        _logger.LogInformation($"Успешно получено количество товара: {count} для {product.Shop} --- {product.ProductName}");
                        return count;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Ошибка при парсинге товара: {product.Shop} --- {product.ProductName}");
                    return "Не удалось спарсить";
                }
            }
        }
    }
}
