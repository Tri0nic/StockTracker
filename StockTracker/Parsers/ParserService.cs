using OpenQA.Selenium;
using StockTracker.Models;
using System.Collections.Generic;

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
            { "Мосигра", new MosigraParser() },
            { "Hobby Games", new HobbyGamesParser() }
        };
        }

        public async Task<IEnumerable<Product>> ParseProducts(IEnumerable<Product> products)
        {
            var availableProducts = new List<Product>();

            foreach (var product in products)
            {
                await Console.Out.WriteLineAsync($"\nНачали парсинг! {product.ProductName}\n");
                if (await ParseProduct(product))
                {
                    await Console.Out.WriteLineAsync($"\nТОВАР ПОЯВИЛСЯ НА САЙТЕ: {product.Shop}, {product.ProductName}\n");
                    availableProducts.Add(product);
                }
            }
            return availableProducts;
        }

        public async Task<bool> ParseProduct(Product product)
        {
            if (_parsers.TryGetValue(product.Shop, out var parser))
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

        public static void EnterText(IWebDriver driver, string xpath, string text)
        {
            var element = driver.FindElement(By.XPath(xpath));
            element.Clear();
            element.SendKeys(text);
        }
        #endregion
    }
}
