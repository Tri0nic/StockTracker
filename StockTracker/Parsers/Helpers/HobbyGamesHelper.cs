using OpenQA.Selenium;
using static StockTracker.Services.ParsersServices.JavaScriptService;
using static StockTracker.Services.ParsersServices.SeleniumService;

namespace StockTracker.Parsers.Helpers
{
    public abstract class HobbyGamesHelper
    {
        public static string CountProducts(IWebDriver driver, ILogger logger)
        {
            OpenShopsMenue(driver, logger);
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
                    if (!shop.Text.Contains('-'))
                    {
                        count += int.Parse((shop.Text).Split(' ')[0]);
                    }
                }

                return count;
            }
        }

        private static void OpenShopsMenue(IWebDriver driver, ILogger logger)
        {
            var optionsBar = driver.FindElements(By.XPath("//li[@class='flat-tab-nav__item ui-tabs-tab ui-corner-top ui-state-default ui-tab']"));
            var availability = optionsBar.Count() + 1;

            ClickElement(driver, $"//a[@id='ui-id-{availability}']");
            logger.LogInformation($"Открыли список магазинов");
        }
    }
}
