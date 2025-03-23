using StockTracker.Services.ParsersServices;
using static StockTracker.Services.ParsersServices.SeleniumService;
using static StockTracker.Parsers.Helpers.MosigraHelper;
using StockTracker.Models;

namespace StockTracker.Parsers
{
    public class MosigraParser : IParser
    {
        public string ShopName => "Мосигра";

        private readonly ProxyService _proxyService;
        private readonly string _driverDirectory;
        private readonly ILogger<MosigraParser> _logger;

        public MosigraParser(ProxyService proxyService, string driverDirectory, ILogger<MosigraParser> logger)
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

                    ChooseRegion(driver, _logger);

                    if (!IsElementAvailable(driver, "//*[@id=\"app-main\"]/main/section[1]/article/section[1]/noindex/div/span[2]/b"))
                    {
                        _logger.LogWarning($"Товар отсутствует в наличии");
                        return "Нет в наличии";
                    }
                    else
                    {
                        string count = CountProducts(driver, _logger);
                        _logger.LogInformation($"Успешно получено количество товара: {count}");
                        return count;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Ошибка при парсинге товара");
                    return "Не удалось спарсить";
                }
            }
        }
    }
}
