using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using static StockTracker.Parsers.ParserService;

namespace StockTracker.Parsers
{
    public class MosigraParser : IParser
    {
        public async Task<bool> Parse(string url)
        {
            try
            {
                #region Selenium

                IWebDriver driver = new ChromeDriver();
                driver.Url = url;

                try
                {
                    // открываем список городов
                    ClickElement(driver, "//*[@id=\"app-main\"]/header/div[2]/div/div/div[1]/button");
                    
                    // выбираем Москву
                    ClickElement(driver, "//*[@id=\"app-main\"]/header/div[1]/div/ul/li[1]/span/a");

                    string IsAvailable = (driver.FindElement(By.XPath("//*[@id=\"app-main\"]/main/section[1]/article/section[1]/noindex/div/span[2]/b"))).Text;
                    return false;
                }
                catch (Exception)
                {
                    return true;
                }

                #endregion
            }
            catch 
            {
                await Console.Out.WriteLineAsync($"Не удалось подключиться к сайту магазина по ссылке: {url}");
            }

            return false;
        }
    }
}
