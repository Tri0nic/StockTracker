using OpenQA.Selenium;
using static StockTracker.Services.ParsersServices.JavaScriptService;
using static StockTracker.Services.ParsersServices.SeleniumService;

namespace StockTracker.Parsers.Helpers
{
    public abstract class MosigraHelper
    {
        public static void ChooseRegion(IWebDriver driver, ILogger logger)
        {
            ClickElement(driver, "//*[@id=\"app-main\"]/header/div[2]/div/div/div[1]/button"); // открываем список городов
            ClickElement(driver, "//*[@id=\"app-main\"]/header/div[1]/div/ul/li[1]/span/a"); // выбираем Москву

            logger.LogInformation("Выбрали город");
        }

        public static string CountProducts(IWebDriver driver, ILogger logger)
        {
            JSHumanSimulation(driver);

            JSClickElement(driver, "//button[@class='btn btn-block btn-cart btn-cart--full-fill buy__button to-cart']"); // Добавление в корзину
            JSClickElement(driver, "//button[@class='btn btn-block btn-cart btn-cart--full-fill buy__button icon-in-cart']"); // Переход в корзину
            JSClickElement(driver, "//a[@class='btn btn-change-step btn-red btn-order']"); // Перейти к оформлению

            return IterativeProductCount(driver).ToString();
        }

        private static int IterativeProductCount(IWebDriver driver)
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
                    if (CountAvailableShops(shop, ref numberOfShops, ref count) == 0)
                    {
                        return count;
                    }
                }

                JSClickElement(driver, "//a[@data-action='plus']"); // Увеличиваем число в корзине
            }
        }

        private static int CountAvailableShops(IWebElement shop, ref int numberOfShops, ref int count)
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
