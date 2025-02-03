using OpenQA.Selenium;
using static StockTracker.Services.ParsersServices.JavaScriptService;
using static StockTracker.Services.ParsersServices.SeleniumService;

namespace StockTracker.Parsers.Helpers
{
    public abstract class HobbyGamesHelper
    {
        public static string CountProducts(IWebDriver driver)
        {
            ClickElement(driver, "//a[@id='ui-id-5']");
            return CountAvailableShops(driver).ToString();
        }

        private static int CountAvailableShops(IWebDriver driver)
        {
            var count = 0;
            while (true)
            {
                JSHumanSimulation(driver);
                var elements = driver.FindElements(By.XPath("//p[@class='product-stocks__stock--qty']")); // Получаем список магазинов
                var numberOfShops = elements.Count();

                foreach (var shop in elements)
                {
                    count += int.Parse((shop.Text).Split(' ')[0]);
                }

                return count;
            }
        }
    }
}
