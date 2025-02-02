using OpenQA.Selenium;
using static StockTracker.Services.ParsersServices.JavaScriptService;
using static StockTracker.Services.ParsersServices.SeleniumService;

namespace StockTracker.Parsers.Helpers
{
    public abstract class YandexMarketHelper
    {
        public static int CountProducts(IWebDriver driver)
        {
            CloseUncessaryWindows(driver);

            ClickElement(driver, "//button[@data-auto='cartButton']");
            JSHumanSimulation(driver);

            ClickElement(driver, "//*[@id=\"CART_ENTRY_POINT_ANCHOR\"]/a");
            JSHumanSimulation(driver);

            EnterText(driver, "//*[@id=\"/content/page/fancyPage/@chef\\/cart\\/CartLayout/@chef\\/cart\\/ChefCartList/@light\\/SlotsTheCreator/MARKET/@chef\\/cart\\/CartLazyWrapper/lazy/initialContent/slots/MARKET_0/list/0_MARKET/addToCartButton\"]/div/div/span/input", "10000");

            ClickElement(driver, "//button[@aria-label='Увеличить']");

            return int.Parse(GetAttribute(driver, "//input[@aria-label='Количество товара']"));
        }

        private static void CloseUncessaryWindows(IWebDriver driver)
        {
            try
            {
                var closeButton = driver.FindElement(By.XPath("//div[@aria-label='Закрыть']"));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", closeButton);
            }
            catch (Exception)
            {
                //Заменить на Log
                Console.WriteLine("Всплывающих окон не обнаружено!");
            }
        }
    }
}
