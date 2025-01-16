using OpenQA.Selenium;
using StockTracker.Models;

namespace StockTracker.Parsers
{
    public class ParserService
    {
        #region Паттерн стратегия
        private readonly Dictionary<string, IParser> _parsers;

        public ParserService()
        {
            _parsers = new Dictionary<string, IParser>
        {
            { "Яндекс Маркет", new YandexMarketParser() },
            { "Мосигра", new MosigraParser() }
        };
        }

        public async Task<bool> ParseProducts(IEnumerable<Product> products)
        {
            foreach (var product in products)
            {
                if (await ParseProduct(product))
                    return true;
            }
            return false;
        }

        public async Task<bool> ParseProduct(Product product)
        {
            if (_parsers.TryGetValue(product.Shop, out var parser)) // Заменить Shop на ProductName
            {
                return await parser.Parse(product.Link);
            }
            else
            {
                throw new NotImplementedException($"Парсинг для магазина {product.Shop} не реализован.");
            }
        }
        #endregion

        #region Вспомогательные методы
        public static void ClickElement(IWebDriver driver, string xpath)
        {
            var element = driver.FindElement(By.XPath(xpath));
            element.Click();
        }
        #endregion
    }
}
