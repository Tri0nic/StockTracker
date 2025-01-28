using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace StockTracker.Services.ParsersServices
{
    public abstract class SeleniumService
    {
        #region Вспомогательные методы

        public static IWebDriver CreateWebDriver(ProxyService proxyService, string url)
        {
            try
            {
                var chromeOptions = proxyService.GetRandomProxy();
                var driver = new ChromeDriver(chromeOptions);
                driver.Url = url;
                return driver;
            }
            catch (Exception)
            {
                Console.WriteLine($"Не удалось подключиться к сайту по ссылке: {url}");
                throw;
            }
        }

        public static bool IsElementAvailable(IWebDriver driver, string xpath)
        {
            try
            {
                Thread.Sleep(5000);
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
            try
            {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                var element = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.XPath(xpath)));
                element.Click();
            }
            catch (Exception)
            {
                Console.WriteLine("Сайт не отвечает более 10 секунд или другая ошибка");
                throw;
            }
        }

        public static string GetText(IWebDriver driver, string xpath)
        {
            return driver.FindElement(By.XPath(xpath)).Text;
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
