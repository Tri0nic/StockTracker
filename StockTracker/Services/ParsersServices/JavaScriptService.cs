using OpenQA.Selenium;

namespace StockTracker.Services.ParsersServices
{
    public class JavaScriptService
    {
        public static void JSHumanSimulation(IWebDriver driver)
        {
            for (int i = 0; i < 5; i++)
            {
                ((IJavaScriptExecutor)driver).ExecuteScript($"window.scrollBy(0, {i * 100});");
                Thread.Sleep(new Random().Next(100, 150));
            }
            for (int i = 4; i >= 0; i--)
            {
                ((IJavaScriptExecutor)driver).ExecuteScript($"window.scrollBy(0, {-i * 100});");
                Thread.Sleep(new Random().Next(100, 150));
            }
        }

        public static void JSClickElement(IWebDriver driver, string xpath)
        {
            var element = driver.FindElement(By.XPath(xpath));

            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            js.ExecuteScript("arguments[0].click();", element);

            Thread.Sleep(new Random().Next(2000, 3001));
        }
    }
}
