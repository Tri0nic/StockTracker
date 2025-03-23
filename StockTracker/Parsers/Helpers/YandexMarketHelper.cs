using OpenQA.Selenium;
using static StockTracker.Services.ParsersServices.JavaScriptService;
using static StockTracker.Services.ParsersServices.SeleniumService;

namespace StockTracker.Parsers.Helpers
{
    public abstract class YandexMarketHelper
    {
        public static string CountProducts(IWebDriver driver, ILogger logger)
        {
            CloseUncessaryWindows(driver, logger);

            ClickElement(driver, "//button[@data-auto='cartButton']");
            JSHumanSimulation(driver);

            ClickElement(driver, "//*[@id=\"CART_ENTRY_POINT_ANCHOR\"]/a");
            JSHumanSimulation(driver);

            EnterText(driver, "//*[@id=\"/content/page/fancyPage/@chef\\/cart\\/CartLayout/@chef\\/cart\\/ChefCartList/@light\\/SlotsTheCreator/MARKET/@chef\\/cart\\/CartLazyWrapper/lazy/initialContent/slots/MARKET_0/list/0_MARKET/addToCartButton\"]/div/div/span/input", "10000");

            ClickElement(driver, "//button[@aria-label='Увеличить']");

            return GetAttribute(driver, "//input[@aria-label='Количество товара']");
        }

        private static void CloseUncessaryWindows(IWebDriver driver, ILogger logger)
        {
            try
            {
                var closeButton = driver.FindElement(By.XPath("//div[@aria-label='Закрыть']"));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", closeButton);
                logger.LogDebug($"Закрыто всплывающее окно.");
            }
            catch (Exception)
            {
                logger.LogDebug("Всплывающих окон не обнаружено.");
            }
        }
    }
}
