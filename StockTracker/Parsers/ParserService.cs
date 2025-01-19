using Microsoft.CodeAnalysis;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using StockTracker.Data;
using StockTracker.Models;
using System.Collections.Generic;

namespace StockTracker.Parsers
{
    public class ParserService
    {
        #region Паттерн стратегия
        private readonly StockTrackerContext _context;
        private readonly ProxyService _proxyService;
        private readonly Dictionary<string, IParser> _parsers;

        public ParserService(ProxyService proxyService, StockTrackerContext context)
        {
            _proxyService = proxyService;
            _context = context;
            _parsers = new Dictionary<string, IParser>
        {
            { "Яндекс Маркет", new YandexMarketParser(_proxyService) }//,
            //{ "Мосигра", new MosigraParser() },
            //{ "Hobby Games", new HobbyGamesParser() }
        };
        }

        public async Task<IEnumerable<Product>> ParseProducts(IEnumerable<Product> products)
        {
            var availableProducts = new List<Product>();

            foreach (var product in products)
            {
                await Console.Out.WriteLineAsync($"\nНачали парсинг! {product.ProductName}\n");
                var parserResult = ParseProduct(product).Result;
                product.ProductCount = parserResult;
                availableProducts.Add(product);
            }
            return availableProducts;
        }

        public async Task<string> ParseProduct(Product product)
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

        public static bool IsAvailable(IWebDriver driver, string xpath)
        {
            try
            {
                var isAvailable = driver.FindElement(By.XPath(xpath)).Text;
                return false;
            }
            catch (NoSuchElementException)
            {
                return true;
            }
        }

        public static void ClickElement(IWebDriver driver, string xpath)
        {
            var element = driver.FindElement(By.XPath(xpath));
            element.Click();
            Thread.Sleep(new Random().Next(2000, 3001));
        }

        public static string GetText(IWebDriver driver, string xpath)
        {
            return (driver.FindElement(By.XPath(xpath))).Text;
        }

        public static void HumanSimulation(IWebDriver driver)
        {
            for (int i = 0; i < 5; i++)
            {
                ((IJavaScriptExecutor)driver).ExecuteScript($"window.scrollBy(0, {i * 100});");
                Thread.Sleep(new Random().Next(250, 500));
            }
            for (int i = 4; i >= 0; i--)
            {
                ((IJavaScriptExecutor)driver).ExecuteScript($"window.scrollBy(0, {-i * 100});");
                Thread.Sleep(new Random().Next(250, 500));
            }
        }

        public static string GetAttribute(IWebDriver driver, string xpath)
        {
            return driver.FindElement(By.XPath(xpath)).GetAttribute("value");
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
