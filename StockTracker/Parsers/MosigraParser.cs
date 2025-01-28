using Microsoft.CodeAnalysis;
using OpenQA.Selenium;
using StockTracker.Services.ParsersServices;
using static StockTracker.Services.ParsersServices.JavaScriptService;
using static StockTracker.Services.ParsersServices.SeleniumService;

namespace StockTracker.Parsers
{
    public class MosigraParser : IParser
    {
        public string ShopName => "Мосигра";

        private readonly ProxyService _proxyService;

        public MosigraParser(ProxyService proxyService)
        {
            _proxyService = proxyService;
        }

        public async Task<string> Parse(string url)
        {
            using (var driver = CreateWebDriver(_proxyService, url))
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

        private void ChooseRegion(IWebDriver driver)
        {
            ClickElement(driver, "//*[@id=\"app-main\"]/header/div[2]/div/div/div[1]/button"); // открываем список городов
            ClickElement(driver, "//*[@id=\"app-main\"]/header/div[1]/div/ul/li[1]/span/a"); // выбираем Москву
        }

        public int CountProducts(IWebDriver driver)
        {
            JSHumanSimulation(driver);

            JSClickElement(driver, "//button[@class='btn btn-block btn-cart btn-cart--full-fill buy__button to-cart']"); // Добавление в корзину
            JSClickElement(driver, "//button[@class='btn btn-block btn-cart btn-cart--full-fill buy__button icon-in-cart']"); // Переход в корзину
            JSClickElement(driver, "//a[@class='btn btn-change-step btn-red btn-order']"); // Перейти к оформлению

            return IterativeProductCount(driver);
        }

        private int IterativeProductCount(IWebDriver driver)
        {
            var count = 0;
            while (true)
            {
                JSClickElement(driver, "//input[@class='visually-hidden']"); //Нажимаем на кнопку самовывоза
                JSHumanSimulation(driver);

                var elements = driver.FindElements(By.XPath("//div[@class='row delivery__methods__item  ']")); // Получаем список магазинов

                var numberOfShops = elements.Count();
                
                foreach (var shop in elements)
                {
                    if (CountAvailableShops(shop, numberOfShops, ref count) == 0)
                    {
                        return count;
                    }
                }

                JSClickElement(driver, "//a[@data-action='plus']"); // Увеличиваем число в корзине
            }
        }

        private int CountAvailableShops(IWebElement shop, int numberOfShops, ref int count) 
        {
            try
            {
                shop.FindElement(By.XPath(".//div[text()='Отсюда можно забрать только часть заказа']"));
                numberOfShops--;
            }
            catch (Exception)
            {
                count++;
            }

            return numberOfShops;
        }
    }
}
