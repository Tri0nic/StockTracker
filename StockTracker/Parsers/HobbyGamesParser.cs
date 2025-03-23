using Microsoft.Extensions.Logging;
using StockTracker.Models;
using StockTracker.Services.ParsersServices;
using static StockTracker.Parsers.Helpers.HobbyGamesHelper;
using static StockTracker.Services.ParsersServices.SeleniumService;

namespace StockTracker.Parsers
{
    public class HobbyGamesParser : IParser
    {
        public string ShopName => "Hobby Games";

        private readonly ProxyService _proxyService;
        private readonly string _driverDirectory;
        private readonly ILogger<HobbyGamesParser> _logger;

        public HobbyGamesParser(ProxyService proxyService, string driverDirectory, ILogger<HobbyGamesParser> logger)
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
                    if (IsElementAvailable(driver, "//a[@id='ui-id-5']"))
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
